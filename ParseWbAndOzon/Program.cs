using System.Collections.ObjectModel;
using System.Diagnostics;
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
        ReadOnlyCollection<IWebElement> fixedPrice = null;
        FirefoxOptions options = new FirefoxOptions();
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        try
        {
            driver.Navigate().GoToUrl("https://www.wildberries.ru/catalog/0/search.aspx?search=msi");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //Thread.Sleep(100000000);
            //Console.WriteLine(driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div/div[2]/div/div/div[1]/div/span/span[1]")).Text);
            driver.Manage().Window.Maximize();
            int y = 75;
            for (int timer = 0; timer < 16; timer++)
            {
                js.ExecuteScript($"window.scrollBy(0,{y.ToString()})");
                y += 75;
                Thread.Sleep(400);
            }

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var price = driver.FindElements(By.TagName("article"));

            Console.Clear();
            if (price.Count > 100)
            {
                fixedPrice = RemoveElem(price);
                PrintInfo(fixedPrice);
            }
            else
            {
                PrintInfo(price);
            }
            
            IWebElement button = driver.FindElement(By.LinkText("Следующая страница"));
            button.Click();
        }
        finally
        {
            driver.Quit();
        }
    }

    public static ReadOnlyCollection<IWebElement> RemoveElem(ReadOnlyCollection<IWebElement> price)
    {
        List<IWebElement> list = price.ToList();
        int listLenght = list.Count;
        for (int i = listLenght - 1; i > 99; i--)
        {
            list.Remove(list[i]);
        }

        return new ReadOnlyCollection<IWebElement>(list);
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
                    webElement.FindElement(By.CssSelector($"#{id
                    } > div > div.product-card__middle-wrap > h2 > span.product-card__brand")).Text,
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
            Console.WriteLine(count);
            Console.WriteLine(product.ToString());
            count++;
        }
    }
}