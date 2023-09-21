using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace ParseWbAndOzon.Parsers;

public class WbParser : Parser<Product>
{
    public WbParser(WebDriver driver, string productName) : base(driver, productName) { }
    
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
        _driver.Navigate().GoToUrl($"https://www.wildberries.ru/catalog/0/search.aspx?search={_productName}");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    protected override bool CheckNextPage()
    {
        try
        {
            _driver.FindElement(By.LinkText("Следующая страница")).Click();
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
            var catalog = _driver.FindElement(By.CssSelector("#catalog > div > div.catalog-page__main.new-size"));
            Products.AddRange(ProductToModel(catalog.FindElements(By.TagName("article"))));
        } while (CheckNextPage());
    }

    protected override List<Product> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        Console.WriteLine(elements.Count);
        List<Product> products = new();
        foreach (var element in elements)
        {
            var id = element.GetAttribute("id");
            var product = new Product
            {
                PriceWithSale = element
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > ins")).Text,
                Price = element
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > del")).Text,
                Brand =
                    element.FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__brand")).Text,
                Name =
                    element.FindElement(
                            By.CssSelector(
                                $"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__name"))
                        .Text,
                Rating = element
                    .FindElement(By.CssSelector(
                        $"#{id
                        } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.address-rate-mini.address-rate-mini--sm"
                    ))
                    .Text,
                AmountRewiew =
                    element.FindElement(By.CssSelector($"#{id
                    } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.product-card__count"))
                        .Text,
                Url = $"https://www.wildberries.ru/catalog/{id.Replace("c", "")}/detail.aspx"
            };
            products.Add(product);
        }

        return products;
    }
}