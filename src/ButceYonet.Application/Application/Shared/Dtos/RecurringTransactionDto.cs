using ButceYonet.Application.Domain.Enums;

namespace ButceYonet.Application.Application.Shared.Dtos;

public class RecurringTransactionDto
{
    public int Id { get; set; }
    public NotebookDto Notebook { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public DateTime? NextOccurrence { get; set; }
    public TransactionDto Transaction { get; set; }
}