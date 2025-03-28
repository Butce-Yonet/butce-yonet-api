using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.NonCategorizedTransactionReports.GetNonCategorizedTransactionReport;

public class GetNonCategorizedTransactionReportQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int? CurrencyId { get; set; }
    public TransactionTypes TransactionTypes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public override string ToString()
    {
        return $"DotBoil:ButceYonet:Report:NonCategorizedTransaction:{NotebookId}-{CurrencyId}-{TransactionTypes}-{StartDate}-{EndDate}";
    }
}