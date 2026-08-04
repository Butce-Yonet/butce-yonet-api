using System.Text.Json;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.EFCore;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ButceYonet.Application.Infrastructure.Jobs;

public class NotebookLabelToNotebookLabelV2 : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public NotebookLabelToNotebookLabelV2(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var sp = scope.ServiceProvider;

        var parameterManager = sp.GetRequiredService<IParameterManager>();
        var isActive = await parameterManager.GetParameterValue<bool>(nameof(NotebookLabelToNotebookLabelV2), "IsActive");

        if (!isActive)
            return;

        var defaultLabelRepository = sp.GetRequiredService<IRepository<DefaultLabel, ButceYonetDbContext>>();
        var notebookLabelRepository = sp.GetRequiredService<IRepository<NotebookLabel, ButceYonetDbContext>>();
        var notebookUserRepository = sp.GetRequiredService<IRepository<NotebookUser, ButceYonetDbContext>>();
        var userLabelRepository = sp.GetRequiredService<IRepository<UserLabel, ButceYonetDbContext>>();
        var mappingRepository = sp.GetRequiredService<IRepository<NotebookLabelToUserLabel, ButceYonetDbContext>>();
        var transactionRepository = sp.GetRequiredService<IRepository<Transaction, ButceYonetDbContext>>();
        var transactionV2Repository = sp.GetRequiredService<IRepository<TransactionV2, ButceYonetDbContext>>();
        var transactionLabelRepository = sp.GetRequiredService<IRepository<TransactionLabel, ButceYonetDbContext>>();
        var transactionLabelV2Repository = sp.GetRequiredService<IRepository<TransactionLabelV2, ButceYonetDbContext>>();
        var categorizedReportRepository = sp.GetRequiredService<IRepository<CategorizedTransactionReport, ButceYonetDbContext>>();
        var categorizedReportV2Repository = sp.GetRequiredService<IRepository<CategorizedTransactionReportV2, ButceYonetDbContext>>();
        var recurringTransactionRepository = sp.GetRequiredService<IRepository<RecurringTransaction, ButceYonetDbContext>>();

        // 1. Default Labels → UserLabels (UserId = null)
        var defaultLabels = await defaultLabelRepository.GetAll().ToListAsync();

        var existingSystemLabels = await userLabelRepository.GetAll()
            .Where(ul => ul.UserId == null)
            .ToListAsync();

        foreach (var defaultLabel in defaultLabels)
        {
            if (existingSystemLabels.Any(ul => ul.Name == defaultLabel.Name))
                continue;

            await userLabelRepository.AddAsync(new UserLabel
            {
                UserId = null,
                Name = defaultLabel.Name,
                ColorCode = defaultLabel.ColorCode
            });
        }

        await userLabelRepository.SaveChangesAsync();

        // Refresh after insert
        existingSystemLabels = await userLabelRepository.GetAll()
            .Where(ul => ul.UserId == null)
            .ToListAsync();

        var defaultLabelNames = defaultLabels.Select(dl => dl.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // 2+3. NotebookLabels → UserLabels + NotebookLabelToUserLabel mapping
        var notebookLabels = await notebookLabelRepository.GetAll().ToListAsync();

        var existingMappedIds = (await mappingRepository.GetAll()
            .Select(m => m.NotebookLabelId)
            .ToListAsync())
            .ToHashSet();

        // default user cache: notebookId → userId
        var defaultUserCache = new Dictionary<int, int?>();

        foreach (var notebookLabel in notebookLabels)
        {
            if (existingMappedIds.Contains(notebookLabel.Id))
                continue;

            UserLabel? targetUserLabel;

            if (defaultLabelNames.Contains(notebookLabel.Name))
            {
                targetUserLabel = existingSystemLabels.FirstOrDefault(ul => ul.Name == notebookLabel.Name);
            }
            else
            {
                if (!defaultUserCache.TryGetValue(notebookLabel.NotebookId, out var defaultUserId))
                {
                    defaultUserId = await notebookUserRepository
                        .GetAll()
                        .Where(nu => nu.NotebookId == notebookLabel.NotebookId && nu.IsDefault)
                        .Select(nu => (int?)nu.UserId)
                        .FirstOrDefaultAsync();

                    defaultUserCache[notebookLabel.NotebookId] = defaultUserId;
                }

                if (defaultUserId is null)
                    continue;

                targetUserLabel = await userLabelRepository.GetAll()
                    .FirstOrDefaultAsync(ul => ul.Name == notebookLabel.Name && ul.UserId == defaultUserId);

                if (targetUserLabel is null)
                {
                    targetUserLabel = new UserLabel
                    {
                        UserId = defaultUserId,
                        Name = notebookLabel.Name,
                        ColorCode = notebookLabel.ColorCode
                    };
                    await userLabelRepository.AddAsync(targetUserLabel);
                    await userLabelRepository.SaveChangesAsync();
                }
            }

            if (targetUserLabel is null)
                continue;

            await mappingRepository.AddAsync(new NotebookLabelToUserLabel
            {
                NotebookLabelId = notebookLabel.Id,
                UserLabelId = targetUserLabel.Id
            });
        }

        await mappingRepository.SaveChangesAsync();

        // Build mapping dictionary: NotebookLabelId → UserLabelId
        var mappingDict = (await mappingRepository.GetAll().ToListAsync())
            .ToDictionary(m => m.NotebookLabelId, m => m.UserLabelId);

        // 4. Transactions → TransactionsV2
        var existingV2Lookup = (await transactionV2Repository.GetAll()
            .Select(tv2 => new { tv2.Id, tv2.ExternalId, tv2.NotebookId })
            .ToListAsync())
            .ToDictionary(x => $"{x.ExternalId}|{x.NotebookId}", x => x.Id);

        var transactions = await transactionRepository.GetAll().ToListAsync();

        // Transaction.Id → TransactionV2.Id
        var transactionIdToV2Id = new Dictionary<int, int>();

        foreach (var t in transactions)
        {
            var key = $"{t.ExternalId}|{t.NotebookId}";
            if (existingV2Lookup.TryGetValue(key, out var existingV2Id))
            {
                transactionIdToV2Id[t.Id] = existingV2Id;
                continue;
            }

            var tv2 = new TransactionV2
            {
                NotebookId = t.NotebookId,
                BankAccountId = t.BankAccountId,
                ExternalId = t.ExternalId,
                Name = t.Name,
                Description = t.Description,
                Amount = t.Amount,
                CurrencyId = t.CurrencyId,
                TransactionType = t.TransactionType,
                IsMatched = t.IsMatched,
                IsProceed = t.IsProceed,
                TransactionDate = t.TransactionDate
            };

            await transactionV2Repository.AddAsync(tv2);
            await transactionV2Repository.SaveChangesAsync();

            transactionIdToV2Id[t.Id] = tv2.Id;
        }

        // 5. TransactionLabels → TransactionLabelV2 (with TransactionV2Id)
        var existingV2Keys = (await transactionLabelV2Repository.GetAll()
            .Select(tlv2 => new { tlv2.TransactionId, tlv2.UserLabelId })
            .ToListAsync())
            .Select(x => $"{x.TransactionId}|{x.UserLabelId}")
            .ToHashSet();

        var transactionLabels = await transactionLabelRepository.GetAll().ToListAsync();

        foreach (var tl in transactionLabels)
        {
            if (!mappingDict.TryGetValue(tl.NotebookLabelId, out var userLabelId))
                continue;

            if (existingV2Keys.Contains($"{tl.TransactionId}|{userLabelId}"))
                continue;

            transactionIdToV2Id.TryGetValue(tl.TransactionId, out var transactionV2Id);

            await transactionLabelV2Repository.AddAsync(new TransactionLabelV2
            {
                TransactionId = tl.TransactionId,
                TransactionV2Id = transactionV2Id == 0 ? null : transactionV2Id,
                UserLabelId = userLabelId
            });
        }

        await transactionLabelV2Repository.SaveChangesAsync();

        // 5b. Update TransactionV2Id on existing TransactionLabelV2 records that are missing it
        var labelsWithoutV2 = await transactionLabelV2Repository.GetAll()
            .Where(tlv2 => tlv2.TransactionV2Id == null)
            .ToListAsync();

        foreach (var label in labelsWithoutV2)
        {
            if (!transactionIdToV2Id.TryGetValue(label.TransactionId, out var v2Id))
                continue;

            label.TransactionV2Id = v2Id;
            transactionLabelV2Repository.Update(label);
        }

        await transactionLabelV2Repository.SaveChangesAsync();

        // 6. CategorizedTransactionReport → CategorizedTransactionReportV2
        var existingV2Reports = (await categorizedReportV2Repository.GetAll()
            .Select(r => new { r.NotebookId, r.UserLabelId, r.TransactionType, r.CurrencyId, r.Term })
            .ToListAsync())
            .Select(r => $"{r.NotebookId}|{r.UserLabelId}|{(int)r.TransactionType}|{r.CurrencyId}|{r.Term:O}")
            .ToHashSet();

        var categorizedReports = await categorizedReportRepository.GetAll().ToListAsync();

        foreach (var report in categorizedReports)
        {
            if (!mappingDict.TryGetValue(report.NotebookLabelId, out var userLabelId))
                continue;

            var key = $"{report.NotebookId}|{userLabelId}|{(int)report.TransactionType}|{report.CurrencyId}|{report.Term:O}";
            if (existingV2Reports.Contains(key))
                continue;

            await categorizedReportV2Repository.AddAsync(new CategorizedTransactionReportV2
            {
                NotebookId = report.NotebookId,
                UserLabelId = userLabelId,
                TransactionType = report.TransactionType,
                CurrencyId = report.CurrencyId,
                Amount = report.Amount,
                Term = report.Term
            });
        }

        await categorizedReportV2Repository.SaveChangesAsync();

        // 7. RecurringTransactions StateData: List<Transaction> → List<TransactionV2>
        var today = DateTime.UtcNow.Date;
        var recurringTransactions = await recurringTransactionRepository
            .GetAll()
            .Where(rt => !rt.EndDate.HasValue || rt.EndDate.Value.Date >= today)
            .ToListAsync();

        foreach (var rt in recurringTransactions)
        {
            // Skip if already in new format
            using var doc = JsonDocument.Parse(rt.StateData);
            var root = doc.RootElement;
            if (root.ValueKind == JsonValueKind.Array &&
                root.GetArrayLength() > 0 &&
                root[0].TryGetProperty("TransactionLabelsV2", out _))
                continue;

            try
            {
                var oldTransactions = JsonSerializer.Deserialize<List<Transaction>>(rt.StateData);

                if (oldTransactions is null || !oldTransactions.Any())
                    continue;

                var newTransactions = oldTransactions.Select(t => new TransactionV2
                {
                    NotebookId = t.NotebookId,
                    ExternalId = t.ExternalId,
                    Name = t.Name,
                    Description = t.Description,
                    Amount = t.Amount,
                    CurrencyId = t.CurrencyId,
                    TransactionType = t.TransactionType,
                    TransactionDate = t.TransactionDate,
                    IsMatched = t.IsMatched,
                    IsProceed = t.IsProceed,
                    TransactionLabelsV2 = t.TransactionLabels
                        .Where(tl => mappingDict.ContainsKey(tl.NotebookLabelId))
                        .Select(tl => new TransactionLabelV2 { UserLabelId = mappingDict[tl.NotebookLabelId] })
                        .ToList()
                }).ToList();

                rt.StateData = JsonSerializer.Serialize(newTransactions);
                recurringTransactionRepository.Update(rt);
            }
            catch
            {
                // Zaten yeni formatta veya parse edilemeyen kayıt, geç
            }
        }

        await recurringTransactionRepository.SaveChangesAsync();
    }
}