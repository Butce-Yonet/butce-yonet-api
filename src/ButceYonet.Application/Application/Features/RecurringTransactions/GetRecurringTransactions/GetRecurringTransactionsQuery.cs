using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.RecurringTransactions.GetRecurringTransactions;

public class GetRecurringTransactionsQuery : PaginationFilter, IRequest<BaseResponse>
{
}
