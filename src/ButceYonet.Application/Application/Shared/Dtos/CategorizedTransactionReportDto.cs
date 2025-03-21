using ButceYonet.Application.Domain.Entities;
using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class CategorizedTransactionReportDto
{
    public Notebook Notebook { get; set; }
    public NotebookLabel NotebookLabel { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public CurrencyDto Currency { get; set; }
    public decimal Amount { get; set; }
    public DateTime Term { get; set; }
}