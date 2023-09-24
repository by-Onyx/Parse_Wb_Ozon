namespace ParseWbAndOzon;

public record OzonProduct
{
    public required string Name { get; set; }
    public required string Price { get; set; }
    public string? PriceWithSale { get; set; }
    public string? SalePercent { get; set; }
    public string? Rating { get; set; }
    public string? AmountRewiew { get; set; }
    public required string Url { get; set; }
}