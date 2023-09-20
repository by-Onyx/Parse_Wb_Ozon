﻿namespace ParseWbAndOzon;

public record Product
{
    public required string Name { get; set; }
    public required string Brand { get; set; }
    public required string Price { get; set; }
    public string PriceWithSale { get; set; }
    public required string Rating { get; set; }
    public required string AmountRewiew { get; set; }
    public required string Url { get; set; }

    public override string ToString()
    {
        return $"{Name.Replace("/ ", "")};{Brand};{Price};{PriceWithSale};{Rating};{AmountRewiew};{Url}";
    }
}