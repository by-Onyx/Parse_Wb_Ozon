using Microsoft.IdentityModel.Tokens;

namespace ParseWbAndOzon;

public record ProductModel
{
    public required string Name { get; set; }
    public string? Brand { get; set; }
    public required string Price { get; set; }
    public string? PriceWithSale { get; set; }
    public string? Rating { get; set; }
    public string? AmountRewiew { get; set; }
    public required string Url { get; set; }

    public override string ToString()
    {
        return $"{Name.Replace("/ ", "")};" +
               $"{Brand};" +
               $"{Price};" +
               $"{PriceWithSale};" +
               $"{Rating?.Replace('.', ',')};" +
               $"{AmountRewiew};" +
               $"HYPERLINK(\"{Url}\")";
    }
}