using System.Runtime.InteropServices.JavaScript;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("notebook-created")]
public class NotebookCreatedDomainEventConsumer : BaseConsumer<NotebookCreatedDomainEvent>
{
    private IRepository<NotebookLabel, ButceYonetDbContext> _notebookLabelRepository;
    private IRepository<NotebookUser, ButceYonetDbContext> _notebookUserRepository;
    private IRepository<DefaultLabel, ButceYonetDbContext> _defaultLabelRepository;
    private ICache _cache;

    private readonly IServiceProvider _serviceProvider;
    
    public NotebookCreatedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<NotebookCreatedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        InitializeDependencies(scope);

        var defaultLabels = await _cache.GetOrSetAsync(CacheKeyConstants.DefaultNotebookLabels, async () =>
        {
            return await _defaultLabelRepository.GetAll().ToListAsync();
        }, CacheIntervalConstants.DefaultNotebookLabels);

        if (!defaultLabels.Any())
            return;

        var notebookId = context.Message.Notebook.Id;

        if (notebookId < 1)
        {
            if (!context.Message.Notebook.NotebookUsers.Any(nu => nu.IsDefault))
                return;

            var userId = context.Message.Notebook.NotebookUsers.FirstOrDefault(nu => nu.IsDefault).UserId;
            var notebooks = await _notebookUserRepository
                .GetAll()
                .Where(nu => nu.UserId == userId)
                .Include(nu => nu.Notebook)
                .ThenInclude(nu => nu.NotebookLabels)
                .Select(nu => nu.Notebook)
                .ToListAsync();

            var notebook = notebooks
                .OrderByDescending(n => n.Id)
                .Where(n => !n.NotebookLabels.Any())
                .FirstOrDefault();

            if (notebook is null)
                return;
            
            notebookId = notebook.Id;
        }

        var notebookLabels = defaultLabels.Select(dl => new NotebookLabel
        {
            NotebookId = notebookId,
            Name = dl.Name,
            ColorCode = dl.ColorCode
        }).ToList();

        await _notebookLabelRepository.AddRangeAsync(notebookLabels);
        await _notebookLabelRepository.SaveChangesAsync();
    }

    private void InitializeDependencies(IServiceScope scope)
    {
        _notebookLabelRepository = scope.ServiceProvider.GetService<IRepository<NotebookLabel, ButceYonetDbContext>>();
        _notebookUserRepository = scope.ServiceProvider.GetService<IRepository<NotebookUser, ButceYonetDbContext>>();
        _defaultLabelRepository = scope.ServiceProvider.GetService<IRepository<DefaultLabel, ButceYonetDbContext>>();
        _cache = scope.ServiceProvider.GetService<ICache>();
    }
}