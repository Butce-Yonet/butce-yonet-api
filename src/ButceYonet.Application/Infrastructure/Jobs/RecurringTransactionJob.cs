using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.MailTemplates;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil;
using DotBoil.Configuration;
using DotBoil.Email;
using DotBoil.Email.Configuration;
using DotBoil.Email.Models;
using DotBoil.EFCore;
using DotBoil.TemplateEngine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using User = ButceYonet.Application.Domain.Entities.User;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class RecurringTransactionJob : BackgroundService
{
    private readonly ILogger<RecurringTransactionJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RecurringTransactionJob(
        ILogger<RecurringTransactionJob> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunDailyAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Recurring transaction job timed out.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Recurring transaction job timed out.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task RunDailyAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        var recurringTransactionRepository = scope.ServiceProvider.GetService<IRepository<RecurringTransaction, ButceYonetDbContext>>();
        var transactionRepository = scope.ServiceProvider.GetService<IRepository<Transaction, ButceYonetDbContext>>();
        var recurringTransactionIntervalsService = scope.ServiceProvider.GetService<IRecurringTransactionIntervalsService>();

        var today = DateTime.UtcNow.Date;
        var startOfDay = today;
        var endOfDay = today.AddDays(1).AddTicks(-1);

        var recurringTransactions = await recurringTransactionRepository
            .GetAll()
            .Where(rt =>
                rt.NextOccurrence.HasValue &&
                rt.NextOccurrence.Value >= startOfDay &&
                rt.NextOccurrence.Value <= endOfDay &&
                (!rt.EndDate.HasValue || rt.EndDate.Value.Date >= today))
            .ToListAsync();

        var processedItemsByNotebook = new Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>>();

        foreach (var recurringTransaction in recurringTransactions)
        {
            var transactions = JsonSerializer.Deserialize<List<Transaction>>(recurringTransaction.StateData);

            if (!transactions.Any())
            {
                recurringTransaction.NextOccurrence =
                    recurringTransactionIntervalsService.CalculateInterval(recurringTransaction.NextOccurrence.Value,
                        recurringTransaction.Frequency, recurringTransaction.Interval);
                
                recurringTransactionRepository.Update(recurringTransaction);
                continue;
            }
            
            var firstTransaction = transactions.FirstOrDefault();

            var transaction = new Transaction
            {
                NotebookId = firstTransaction.NotebookId,
                ExternalId = Guid.NewGuid().ToString(),
                Name = firstTransaction.Name,
                Description = firstTransaction.Description,
                Amount = firstTransaction.Amount,
                CurrencyId = firstTransaction.CurrencyId,
                TransactionType = firstTransaction.TransactionType,
                TransactionDate = DateTime.Now
            };

            transaction.TransactionLabels = new List<TransactionLabel>();
            transaction.TransactionLabels = firstTransaction.TransactionLabels.Select(tl => new TransactionLabel
            {
                NotebookLabelId = tl.NotebookLabelId
            }).ToList();
            
            transaction.IsMatched = transaction.TransactionLabels.Any();

            var transactionCreatedDomainEvent = new TransactionCreatedDomainEvent(transaction);
            transaction.AddEvent(transactionCreatedDomainEvent);
            await transactionRepository.AddAsync(transaction);
            
            recurringTransaction.NextOccurrence =
                recurringTransactionIntervalsService.CalculateInterval(recurringTransaction.NextOccurrence.Value,
                    recurringTransaction.Frequency, recurringTransaction.Interval);
                
            recurringTransactionRepository.Update(recurringTransaction);

            var notebookId = firstTransaction.NotebookId;
            if (notebookId.HasValue && notebookId.Value > 0)
            {
                if (!processedItemsByNotebook.TryGetValue(notebookId.Value, out var list))
                {
                    list = new List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>();
                    processedItemsByNotebook[notebookId.Value] = list;
                }
                list.Add((recurringTransaction.Name, firstTransaction.Description ?? "", firstTransaction.Amount, firstTransaction.CurrencyId, firstTransaction.TransactionType));
            }
        }

        await recurringTransactionRepository.SaveChangesAsync();

        await SendRecurringProcessedNotificationEmailsAsync(scope.ServiceProvider, processedItemsByNotebook, today, cancellationToken);
    }

    private async Task SendRecurringProcessedNotificationEmailsAsync(
        IServiceProvider serviceProvider,
        Dictionary<int, List<(string Name, string Description, decimal Amount, int CurrencyId, TransactionTypes TransactionType)>> processedItemsByNotebook,
        DateTime processedDate,
        CancellationToken cancellationToken)
    {
        if (processedItemsByNotebook.Count == 0)
            return;

        var notebookUserRepository = serviceProvider.GetService<IRepository<NotebookUser, ButceYonetDbContext>>();
        var userRepository = serviceProvider.GetService<IRepository<User, ButceYonetAuthorizationDbContext>>();
        var currencyRepository = serviceProvider.GetService<IRepository<Currency, ButceYonetDbContext>>();
        var razorRenderer = serviceProvider.GetService<IRazorRenderer>();
        var mailSender = serviceProvider.GetService<IMailSender>();

        var currencyMap = await currencyRepository.GetAll()
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        var serverSettings = DotBoilApp.Configuration.GetConfigurations<EmailOptions>();
        var serverSetting = serverSettings.ServerSettings.FirstOrDefault();

        foreach (var (notebookId, items) in processedItemsByNotebook)
        {
            if (items.Count == 0)
                continue;

            var notebookUserIds = await notebookUserRepository
                .Get()
                .Where(nu => nu.NotebookId == notebookId)
                .Select(nu => nu.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var userId in notebookUserIds)
            {
                var user = await userRepository.Get()
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (user is null || string.IsNullOrWhiteSpace(user.Email))
                    continue;

                var templateItems = items.Select(i =>
                {
                    var currency = currencyMap.GetValueOrDefault(i.CurrencyId);
                    var isIncome = i.TransactionType == TransactionTypes.Income;
                    return new ProcessedRecurringItem
                    {
                        Name = i.Name,
                        Description = i.Description,
                        TransactionTypeDisplay = isIncome ? "Gelir" : "Gider",
                        IsIncome = isIncome,
                        Amount = i.Amount,
                        CurrencySymbol = currency?.Symbol ?? "",
                        IsSymbolRight = currency?.IsSymbolRight ?? true
                    };
                }).ToList();

                var model = new RecurringTransactionsProcessedTemplateModel
                {
                    UserName = string.IsNullOrWhiteSpace(user.Name) ? user.Email : $"{user.Name} {user.Surname}".Trim(),
                    ProcessedDate = processedDate,
                    Items = templateItems,
                    Year = DateTime.UtcNow.Year
                };

                try
                {
                    var mailContent = await razorRenderer.RenderAsync("RecurringTransactionsProcessedTemplate", model);
                    await mailSender.SendAsync(serverSetting.Value, new Message
                    {
                        From = new List<string> { serverSetting.Value.EmailAddress },
                        To = new List<string> { user.Email },
                        Attachments = new List<Attachment>(),
                        Body = mailContent,
                        Subject = "Bütçe Yönet - Tekrarlayan gelir-gider işlendi"
                    });
                }
                catch (Exception ex)
                {
                    // Mail gönderimi hata verse bile job tamamlansın; loglama host'ta yapılabilir
                }
            }
        }
    }
}