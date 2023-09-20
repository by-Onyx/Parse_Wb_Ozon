using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;

namespace ParseWbAndOzon;

class Program
{
    public static void Main()
    {
        FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--headless");
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        try
        {
            driver.Navigate().GoToUrl("https://www.wildberries.ru/catalog/0/search.aspx?search=msi+материнская+плата");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //Thread.Sleep(100000000);
            //Console.WriteLine(driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div/div[2]/div/div/div[1]/div/span/span[1]")).Text);
            while (true)
            {
                driver.Manage().Window.Maximize();

                double speed = 0.3;
                int pageHeight = Convert.ToInt32(js.ExecuteScript("return document.body.scrollHeight"));
                double position = 0;
                while (pageHeight - 100 > position)
                {
                    position += speed;
                    js.ExecuteScript($"window.scrollBy(0,{position.ToString(CultureInfo.InvariantCulture)})");
                }

                Console.WriteLine("yep");
                
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                js.ExecuteScript("window.scrollTo(0,document.body.scrollHeight);");
               
                var catalog = driver.FindElement(By.CssSelector("#catalog > div > div.catalog-page__main.new-size"));

                var productCards = catalog.FindElements(By.TagName("article"));
                
                PrintInfo(productCards);

                try
                {
                    IWebElement button = driver.FindElement(By.LinkText("Следующая страница"));
                    button.Click();
                }
                catch
                {
                    break;
                }
            }
        }
        finally
        {
            driver.Quit();
        }
    }

    public static void PrintInfo(ReadOnlyCollection<IWebElement> price)
    {
        Console.WriteLine(price.Count + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        int count = 0;
        foreach (var webElement in price)
        {
            var id = webElement.GetAttribute("id");
            var product = new Product
            {
                PriceWithSale = webElement
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > ins")).Text,
                Price = webElement
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > del")).Text,
                Brand =
                    webElement.FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__brand")).Text,
                Name =
                    webElement.FindElement(
                            By.CssSelector(
                                $"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__name"))
                        .Text,
                Rating = webElement
                    .FindElement(By.CssSelector(
                        $"#{id
                        } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.address-rate-mini.address-rate-mini--sm"
                    ))
                    .Text,
                AmountRewiew =
                    webElement.FindElement(By.CssSelector($"#{id
                    } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.product-card__count"))
                        .Text,
                Url = $"https://www.wildberries.ru/catalog/{id.Replace("c", "")}/detail.aspx"
            };
            Console.WriteLine(product.ToString());
            count++;
        }
    }
}