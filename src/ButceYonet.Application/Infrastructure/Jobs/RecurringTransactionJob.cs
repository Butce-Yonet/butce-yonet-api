using System.Text.Json;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Events;
using ButceYonet.Application.Infrastructure.Data;
using ButceYonet.Application.Infrastructure.Services;
using DotBoil.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        
        var recurringTransactions = await recurringTransactionRepository
            .GetAll()
            .Where(rt =>
                rt.EndDate.Value.Date >= DateTime.Now.Date &&
                rt.NextOccurrence != null &&
                rt.NextOccurrence.Value.Date <= DateTime.Now.Date)
            .ToListAsync();

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
        }

        await recurringTransactionRepository.SaveChangesAsync();
    }
}