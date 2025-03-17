using ButceYonet.Application.Domain.Enums;
using DotBoil.Entities;

namespace ButceYonet.Application.Domain.Entities;

public class RecurringTransaction : BaseEntity
{
    public int NotebookId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RecurringTransactionIntervals Frequency { get; set; }
    public int? Interval { get; set; }
    public DateTime NextOccurrence { get; set; }
    public string StateData { get; set; }
}