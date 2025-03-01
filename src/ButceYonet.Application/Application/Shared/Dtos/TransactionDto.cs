using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class TransactionDto
{
    public int Id { get; set; }
    public int NotebookId { get; set; }
    public string ExternalId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public TransactionTypes TransactionType { get; set; }
    public bool IsMatched { get; set; }
    public bool IsProceed { get; set; }
    public DateTime TransactionDate { get; set; }

    public NotebookDto Notebook { get; set; }
    public CurrencyDto Currency { get; set; }
    public List<NotebookLabelDto> Labels { get; set; }
}