using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace ParseWbAndOzon;

class Program
{
    public static void Main()
    {
        FirefoxOptions options = new FirefoxOptions();
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        var handleLink = "https://www.ozon.ru/search?text=MSI+MPG";
        try
        {
            int i = 0;
            do
            {
                driver = new FirefoxDriver(options);
                driver.Navigate().GoToUrl(handleLink);
                //MSI+MPG+B550+GAMING+PLUS
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                driver.Manage().Window.Maximize();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                i++;
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
                    Console.WriteLine("-----------------------------");
                    Console.WriteLine(ozon.Name);
                    Console.WriteLine(ozon.Price);
                    Console.WriteLine(ozon.PriceWithSale);
                    Console.WriteLine(ozon.SalePercent);
                    Console.WriteLine("-----------------------------");
                    products.Add(ozon);
                }

                Console.WriteLine(products.Count);
                
                IWebElement button = driver.FindElement(By.XPath("//*[@id=\"layoutPage\"]/div[1]/div[2]/div[2]/div[2]/div[3]/div[2]/div/div/div[2]/a"));
                handleLink = button.GetAttribute("href");
                driver.Close();
            } while (i != 3);
        }
        finally
        {
            driver.Quit();
        }
    }

}