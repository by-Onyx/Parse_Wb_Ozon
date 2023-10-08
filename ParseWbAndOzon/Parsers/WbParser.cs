using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ParseWbAndOzon.Parsers;

public class WbParser : Parser
{
    public WbParser(FirefoxDriver driver, string productName) : base(driver, productName)
    {
        handleLink = $"https://www.wildberries.ru/catalog/0/search.aspx?search={_productName}";
    }
    
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
            driver.Quit();
        }
    }
    
    protected override void NavigateToPage()
    {
        driver.Navigate().GoToUrl(handleLink);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    protected override bool CheckNextPage()
    {
        try
        {
            driver.FindElement(By.LinkText("Следующая страница")).Click();
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
            ScrollToPageEnd(100);
            
            var catalog = driver
                .FindElement(By.CssSelector("#catalog > div > div.catalog-page__main.new-size"));
            
            Products.AddRange(ProductToModel(catalog.FindElements(By.TagName("article"))));
            
            GC.Collect();
        } while (CheckNextPage());
    }
    
    protected override List<ProductModel> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        List<ProductModel> products = new();
        foreach (var element in elements)
        {
            var id = element.GetAttribute("id");
            var product = new ProductModel
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