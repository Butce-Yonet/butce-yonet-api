using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class CategorizedTransactionReportDto
{
    public NotebookDto Notebook { get; set; }
    public NotebookLabelDto NotebookLabel { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public CurrencyDto Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime Term { get; set; }
}