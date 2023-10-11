using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace ParseWbAndOzon.Parsers;

public class OzonParser : Parser
{
    private string productPricesCard;
    private int currentPageNumber;
    private Func<string> GetHandleLink;
    public OzonParser(FirefoxDriver driver, FirefoxOptions options, string productName) : base(driver, options, productName)
    {
        currentPageNumber = 1;
        GetHandleLink = () => 
            handleLink = 
                $"https://www.ozon.ru/search/?deny_category_prediction=true&from_global=true&page={currentPageNumber}&sorting=price_desc&text={productName}";
    }
    
    public override void Parse()
    {
        try
        {
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
        //driver = new FirefoxDriver(_options);
        driver.Navigate().GoToUrl(GetHandleLink());
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    protected override bool CheckNextPage()
    {
        try
        {
            driver.FindElement(By.LinkText("Дальше"));
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
            
            var catalog = driver.FindElement(By.CssSelector("#paginatorContent"));
            if (!CheckBrandTag(catalog))
            {
                Console.WriteLine("fuck");
                continue;
            }
            
            currentPageNumber++;
            
            ScrollToPageEnd(50);
            
            var productCardAttribute =
                catalog.FindElement(By.XPath("//*[@id=\"paginatorContent\"]/div/div/div[1]/div[1]"))
                    .GetAttribute("class")
                    .Replace(" ", ".");
            
            productPricesCard =
                catalog.FindElement(By.CssSelector($"#paginatorContent > div > div > div:nth-child(1) > div.{productCardAttribute} > div:nth-child(1) > div"))
                    .GetAttribute("class")
                    .Replace(" ", ".");
            
            Products.AddRange(ProductToModel(catalog.FindElements(By.CssSelector($".{productCardAttribute}"))));

            GC.Collect();
        } while (CheckNextPage());
    }
    
    protected override List<ProductModel> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        List<ProductModel> products = new();
        string price;
        string? priceWithSale = null;
        foreach (var element in elements)
        {
            var priceCard = element
                .FindElement(By.ClassName(productPricesCard)).Text;
            
            var priceInfo = priceCard.Split("\r\n");
            
            if (priceInfo.Length > 1)
            {
                priceWithSale = priceInfo[0];
                price = priceInfo[1];
            }
            else
            {
                price = priceInfo[0];
            }

            string? brand;
            try
            {
                brand = element.FindElement(By.ClassName("tsBody400Small")).Text;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            }
            catch
            {
                brand = null;
            }

            string? rating; 
            string? amountRewiew;
            
            try
            {
                var rewiews = element.FindElements(By.ClassName("v1"));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
                rating = rewiews[0].Text.Trim();
                amountRewiew = rewiews[1].Text;
            }
            catch
            {
                rating = null;
                amountRewiew = null;
            }
            
            var product = new ProductModel
            {
                PriceWithSale = priceWithSale,
                Price = price,
                Brand = brand,
                Name = element.FindElement(By.ClassName("tsBody500Medium")).Text,
                Rating = rating,
                AmountRewiew = amountRewiew,
                Url = element.FindElement(By.TagName("a")).GetAttribute("href")
            };
            products.Add(product);
        }

        return products;
    }

    private bool CheckBrandTag(IWebElement catalog)
    {
        try
        {
            string brand = catalog
                .FindElement(By.ClassName("tsBody400Small"))
                .Text;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            Console.WriteLine(brand);
            if (brand == "Стало дешевле") return false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}