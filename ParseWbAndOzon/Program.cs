using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using ParseWbAndOzon.Parsers;

namespace ParseWbAndOzon;

class Program
{
    public static void Main()
    {
        FirefoxOptions options = new FirefoxOptions();
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        try
        {
            driver.Navigate().GoToUrl("https://www.ozon.ru/search?text=MSI+MPG");
            //MSI+MPG+B550+GAMING+PLUS
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            driver.Manage().Window.Maximize();
            Thread.Sleep(3000);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            var elements = driver.FindElement(By.CssSelector("#paginatorContent"));
            
            var productCardClass =
                elements.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1)"))
                    .GetAttribute("class")
                    .Replace(' ', '.');

            var priceAttribute =
                elements.FindElement(By.CssSelector("#paginatorContent > div > div > div:nth-child(1) > div:nth-child(2)"))
                    .GetAttribute("class")
                    .Replace(' ', '.');

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
            }
            foreach (var product in products)
            {
                Console.WriteLine(product.Name);
                Console.WriteLine(product.Price);
                Console.WriteLine(product.PriceWithSale);
                Console.WriteLine(product.SalePercent);
                Console.WriteLine(product.Url);
                Console.WriteLine();
            }

            Console.WriteLine(products.Count);

            IWebElement button = driver.FindElement(By.LinkText("Дальше"));
            button.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            
            Thread.Sleep(5000);
            
            IWebElement captha = driver.FindElement(By.CssSelector(".ctp-checkbox-label > input:nth-child(1)"));
            captha.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            Thread.Sleep(100000);
            
        }
        finally
        {
            driver.Quit();
        }
    }

}