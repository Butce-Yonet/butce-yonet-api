namespace ButceYonet.Application.Application.Shared.Dtos;

public class CurrencyDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Symbol { get; set; }
    public bool IsSymbolRight { get; set; }
}