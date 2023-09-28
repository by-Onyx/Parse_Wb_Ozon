using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace ParseWbAndOzon;

class Program
{
    public static void Main()
    {
        FirefoxOptions options = new FirefoxOptions();
        FirefoxDriver driver = null;
        var handleLink = "https://www.ozon.ru/search?text=Телевизор+LG";
        try
        {
            
            while (true)
            {
                driver = new FirefoxDriver(options);
                driver.Navigate().GoToUrl(handleLink);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driver.Manage().Window.Maximize();
                //Thread.Sleep(100000000);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                var elements = driver.FindElement(By.CssSelector("#paginatorContent"));
                
                var productCardClass =
                    elements.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1)"))
                        .GetAttribute("class")
                        .Replace(" ", ".");
                
                var priceAttribute =
                    elements.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1) > div:nth-child(2)"))
                        .GetAttribute("class")
                        .Replace(" ", ".");
                
                var productCards = elements
                    .FindElements(By.CssSelector($".{productCardClass}"));
                
                var products = new List<OzonProduct>();
                
                foreach (var productCard in productCards)
                {
                    var priceCard = productCard
                        .FindElement(By.CssSelector(
                            $"#paginatorContent > div > div > div.{productCardClass} > div.{priceAttribute} > div:nth-child(1)"))
                        .Text;
                
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
                        Name = productCard.FindElement(By.ClassName("tsBody500Medium")).Text,
                        Price = price,
                        PriceWithSale = priceWithSale,
                        SalePercent = salePercent,
                        Url = productCard.FindElement(By.TagName("a")).GetAttribute("href")
                    };
                    products.Add(ozon);
                    Console.WriteLine(ozon.Name);
                }
                
                Console.WriteLine(products.Count);
                
                IWebElement button = driver.FindElement(By.CssSelector("a.a2425-a4"));
                handleLink = button.GetAttribute("href");
                driver.Close();
                
                Thread.Sleep(10000);
            } 
        }
        finally
        {
            driver.Quit();
        }
    }

}