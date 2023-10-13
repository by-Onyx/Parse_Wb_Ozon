using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ParseWbAndOzon.Parsers;

public class WbParser : Parser
{
    public WbParser(FirefoxDriver driver, FirefoxOptions options, string productName) : base(driver, options, productName) { }
    
    public override void Parse()
    {
        try
        {
            var brandId = GetBrandId();
            GetHandleLink = () =>
                handleLink =
                    $"https://www.wildberries.ru/catalog/0/search.aspx?page={currentPageNumber}&sort=pricedown&search={_productName}&fbrand={brandId}";

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
        driver.Manage().Cookies.DeleteAllCookies();
        driver.Navigate().GoToUrl(GetHandleLink());
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    protected override string GetBrandId()
    {
        driver.Manage().Cookies.DeleteAllCookies();
        driver.Navigate().GoToUrl($"https://www.wildberries.by/catalog?search={_brand}&tail-location=SNT");
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        driver.FindElement(By.ClassName("card-cell")).Click();
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        return driver.FindElement(By.ClassName("product-header__title"))
            .FindElement(By.TagName("a"))
            .GetAttribute("href")
            .Split("brandpage=")[1]
            .Split("__")[0];
    }
    
    protected override bool CheckNextPage()
    {
        try
        {
            driver.FindElement(By.LinkText("Следующая страница"));
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
            NavigateToPage();
            IWebElement? catalog; 
            try
            {
                catalog = driver.FindElement(By.CssSelector("#catalog > div > div.catalog-page__main.new-size"));
            }
            catch(Exception e)
            {
                driver.Quit();
                driver = new FirefoxDriver(_options);
                continue;
            }
            currentPageNumber++;
            ScrollToPageEnd(100);
            
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

            string priceWithSale;
            try
            {
                priceWithSale = element
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > ins")).Text;
            }
            catch
            {
                priceWithSale = "";
            }
            var product = new ProductModel
            {
                PriceWithSale = priceWithSale,
                Price = element
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > del")).Text,
                Brand = _brand,
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