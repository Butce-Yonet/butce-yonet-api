using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;
using MediatR;

namespace ButceYonet.Application.Application.Features.CategorizedTransactionReports.GetCategorizedTransactionReport;

public class GetCategorizedTransactionReportQuery : IRequest<BaseResponse>
{
    public int NotebookId { get; set; }
    public int? NotebookLabelId { get; set; }
    public TransactionTypes TransactionTypes { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}