using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.MassTransit.Attributes;
using DotBoil.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Consumers;

[Consumer("notebook-deleted")]
public class NotebookDeletedDomainEventConsumer : BaseConsumer<NotebookDeletedDomainEvent>
{
    private readonly IServiceProvider _serviceProvider;
    
    public NotebookDeletedDomainEventConsumer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task ConsumeEvent(ConsumeContext<NotebookDeletedDomainEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();
        var notebookUserRepository = scope.ServiceProvider.GetService<IRepository<NotebookUser, ButceYonetDbContext>>();

        var notebookUsers = await notebookUserRepository
            .GetAll()
            .Where(p => p.NotebookId == context.Message.NotebookId)
            .ToListAsync();
        
        notebookUserRepository.RemoveRange(notebookUsers);
        await notebookUserRepository.SaveChangesAsync();
    }
}