using System.Net;
using AutoMapper;
using ButceYonet.Application.Application.Interfaces;
using ButceYonet.Application.Application.Shared.Dtos;
using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;
using ButceYonet.Application.Infrastructure.Data;
using DotBoil.Caching;
using DotBoil.EFCore;
using DotBoil.Entities;
using DotBoil.Localization;
using DotBoil.Parameter;
using Microsoft.EntityFrameworkCore;

namespace ButceYonet.Application.Application.Features.Transactions.GetCalendar;

public class GetCalendarQueryHandler : BaseHandler<GetCalendarQuery, BaseResponse>
{
    private readonly IRepository<TransactionV2, ButceYonetDbContext> _transactionRepository;

    public GetCalendarQueryHandler(
        ICache cache,
        IUser user,
        IMapper mapper,
        ILocalize localize,
        IParameterManager parameter,
        IUserPlanValidator userPlanValidator,
        IRepository<TransactionV2, ButceYonetDbContext> transactionRepository)
        : base(cache, user, mapper, localize, parameter, userPlanValidator)
    {
        _transactionRepository = transactionRepository;
    }

    public override async Task<BaseResponse> ExecuteRequest(GetCalendarQuery request, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(request.Year, request.Month, 1);
        var endDate = startDate.AddMonths(1).AddTicks(-1);

        var transactions = await _transactionRepository
            .GetAll()
            .Where(t => t.NotebookV2.UserId == _user.Id)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .Include(t => t.NotebookV2)
            .Include(t => t.Currency)
            .Include(t => t.TransactionLabelsV2.Where(tl => !tl.IsDeleted))
            .ThenInclude(tl => tl.UserLabel)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync(cancellationToken);

        var transactionDtos = _mapper.Map<List<TransactionDto>>(transactions);

        var days = transactionDtos
            .GroupBy(t => t.TransactionDate.Date)
            .Select(g =>
            {
                var totalIncome = g.Where(t => t.TransactionType == TransactionTypes.Income).Sum(t => t.Amount);
                var totalExpense = g.Where(t => t.TransactionType == TransactionTypes.Expense).Sum(t => t.Amount);

                return new CalendarDayDto
                {
                    Date = g.Key,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    NetBalance = totalIncome - totalExpense,
                    TransactionCount = g.Count(),
                    PreviewTransactions = g
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(request.PreviewCount)
                        .ToList()
                };
            })
            .OrderBy(d => d.Date)
            .ToList();

        var monthTotalIncome = transactionDtos.Where(t => t.TransactionType == TransactionTypes.Income).Sum(t => t.Amount);
        var monthTotalExpense = transactionDtos.Where(t => t.TransactionType == TransactionTypes.Expense).Sum(t => t.Amount);

        var dto = new CalendarMonthDto
        {
            Year = request.Year,
            Month = request.Month,
            StartDate = startDate,
            EndDate = endDate.Date,
            TotalIncome = monthTotalIncome,
            TotalExpense = monthTotalExpense,
            NetBalance = monthTotalIncome - monthTotalExpense,
            Days = days
        };

        return BaseResponse.Response(dto, HttpStatusCode.OK);
    }
}
