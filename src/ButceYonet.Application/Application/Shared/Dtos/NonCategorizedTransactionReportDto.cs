using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class NonCategorizedTransactionReportDto
{
    public NotebookDto NotebookDto { get; set; }
    public TransactionTypes TransactionTypes { get; set; }
    public CurrencyDto Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime Term { get; set; }
}