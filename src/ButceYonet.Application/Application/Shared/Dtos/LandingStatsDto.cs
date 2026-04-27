namespace ButceYonet.Application.Application.Shared.Dtos;

public class LandingStatsDto
{
    public int TotalNotebooks { get; set; }
    public int NewNotebooksThisMonth { get; set; }
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int TotalTransactions { get; set; }
    public int NewTransactionsLastMonth { get; set; }
    public decimal TotalVolume { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
}