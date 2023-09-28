using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace ParseWbAndOzon.Parsers;

public class OzonParser : Parser<OzonProduct>
{
    public OzonParser(WebDriver driver, string productName) : base(driver, productName) { }

    public override void Parse()
    {
        try
        {
            NavigateToPage();
            GetAllProductsCard();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            _driver.Quit();
        }
    }

    protected override void NavigateToPage()
    {
        _driver.Navigate().GoToUrl($"https://www.ozon.ru/search?text={_productName}");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    protected override bool CheckNextPage()
    {
        try
        {
            _driver.FindElement(By.CssSelector("a.a2425-a4")).Click();
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override void GetAllProductsCard()
    {
        do
        {
            _driver.Manage().Window.Maximize();
            ScrollToPageEnd();
            var page = _driver.FindElement(By.CssSelector("#paginatorContent"));
            var priceCard = GetPriceAttribute(page);
            var productCardClass = GetProductCardClass(page);
            var productCards = GetProductsCards(page, productCardClass);

            Products.AddRange(ProductToRecord(productCards, priceCard, productCardClass));
        } while (CheckNextPage());
    }

    protected override List<OzonProduct> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        throw new NotImplementedException();
    }

    private List<OzonProduct> ProductToRecord(ReadOnlyCollection<IWebElement> elements, string priceAttribute, string productCardClass)
    {
        List<OzonProduct> products = new();

        foreach (var element in elements)
        {
            var priceCard = FindPriceCard(element, priceAttribute, productCardClass);
            string price;
            string? priceWithSale = null;
            string? salePercent = null;

            var priceInfo = priceCard.Split("\n");

            if (priceInfo.Length > 1)
            {
                priceWithSale = priceInfo[0];
                price = priceInfo[1];
                salePercent = priceInfo[2];
            }
            else
            {
                price = priceInfo[0];
            }

            var ozon = new OzonProduct
            {
                Name = element.FindElement(By.ClassName("tsBody500Medium")).Text,
                Price = price,
                PriceWithSale = priceWithSale,
                SalePercent = salePercent,
                Url = element.FindElement(By.TagName("a")).GetAttribute("href")
            };
            products.Add(ozon);
        }

        return products;    
    }

    // private ReadOnlyCollection<IWebElement> PageToCollection(IWebElement page)
    // {
    //     var productCardClass =
    //         page.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1)"))
    //             .GetAttribute("class")
    //             .Replace(" ", ".");
    //
    //     var catalog = page.FindElements(By.CssSelector($"{productCardClass}"));
    //
    //     return catalog;
    // }
    //
    private string GetPriceAttribute(IWebElement priceCard)
    {
        return priceCard.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1) > div:nth-child(2)"))
            .GetAttribute("class")
            .Replace(" ", ".");
    }

    private string GetProductCardClass(IWebElement page)
    {
        return page.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1)"))
            .GetAttribute("class")
            .Replace(" ", ".");
    }

    private ReadOnlyCollection<IWebElement> GetProductsCards(IWebElement page, string productCardClass)
    {
        return page
            .FindElements(By.CssSelector($".{productCardClass}"));
    }

    private string FindPriceCard(IWebElement card, string priceAttribute, string productCardClass)
    {
        return card
            .FindElement(By.CssSelector(
                $"#paginatorContent > div > div > div.{productCardClass} > div.{priceAttribute} > div:nth-child(1)"))
            .Text;
    }
}