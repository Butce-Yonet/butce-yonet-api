using System.Net;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Constants;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Landing.GetLandingStats;

public class GetLandingStatsQueryHandler : IRequestHandler<GetLandingStatsQuery, BaseResponse>
{
    private readonly ICache _cache;
    private readonly IRepository<Notebook, ButceYonetDbContext> _notebookRepository;
    private readonly IRepository<Transaction, ButceYonetDbContext> _transactionRepository;
    private readonly IRepository<User, ButceYonetAuthorizationDbContext> _userRepository;

    public GetLandingStatsQueryHandler(
        ICache cache,
        IRepository<Notebook, ButceYonetDbContext> notebookRepository,
        IRepository<Transaction, ButceYonetDbContext> transactionRepository,
        IRepository<User, ButceYonetAuthorizationDbContext> userRepository)
    {
        _cache = cache;
        _notebookRepository = notebookRepository;
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse> Handle(GetLandingStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await _cache.GetOrSetAsync(
            CacheKeyConstants.LandingStats,
            async () => await FetchStats(cancellationToken),
            CacheIntervalConstants.LandingStats);

        return BaseResponse.Response(stats, HttpStatusCode.OK);
    }

    private async Task<LandingStatsDto> FetchStats(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var thirtyDaysAgo = now.AddDays(-30);

        var totalNotebooks = await _notebookRepository.Get().CountAsync(cancellationToken);
        var newNotebooksThisMonth = await _notebookRepository.Get()
            .Where(n => n.CreateTime >= startOfMonth)
            .CountAsync(cancellationToken);

        var totalUsers = await _userRepository.Get().CountAsync(cancellationToken);
        var newUsersThisMonth = await _userRepository.Get()
            .Where(u => u.CreateTime >= startOfMonth)
            .CountAsync(cancellationToken);

        var totalTransactions = await _transactionRepository.Get().CountAsync(cancellationToken);
        var newTransactionsLastMonth = await _transactionRepository.Get()
            .Where(t => t.CreateTime >= thirtyDaysAgo)
            .CountAsync(cancellationToken);

        var totalIncome = await _transactionRepository.Get()
            .Where(t => t.TransactionType == TransactionTypes.Income)
            .SumAsync(t => t.Amount, cancellationToken);

        var totalExpense = await _transactionRepository.Get()
            .Where(t => t.TransactionType == TransactionTypes.Expense)
            .SumAsync(t => t.Amount, cancellationToken);

        return new LandingStatsDto
        {
            TotalNotebooks = totalNotebooks,
            NewNotebooksThisMonth = newNotebooksThisMonth,
            TotalUsers = totalUsers,
            NewUsersThisMonth = newUsersThisMonth,
            TotalTransactions = totalTransactions,
            NewTransactionsLastMonth = newTransactionsLastMonth,
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalVolume = totalIncome + totalExpense
        };
    }
}