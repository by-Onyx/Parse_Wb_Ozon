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
        int pageNumbers = 1;
        FirefoxOptions options = new FirefoxOptions();
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        try
        {
            driver.Navigate().GoToUrl("https://www.wildberries.ru/catalog/0/search.aspx?search=msi");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            driver.Manage().Window.Maximize();
            for (int i = 1; i <= pageNumbers; i++)
            {
                Thread.Sleep(5000); //На случай, если плохой интернет и страница долго грузится
                int y = 50;
                for (int timer = 0; timer < 19; timer++)
                {
                    js.ExecuteScript($"window.scrollBy(0,{y.ToString()})");
                    y += 50;
                    Thread.Sleep(1000);
                }

                Thread.Sleep(2000);

                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                var price = driver.FindElements(By.TagName("article"));

                Console.Clear();
                if (price.Count > 100)
                {
                    fixedPrice = RemoveElem(price);
                }

                Console.WriteLine(fixedPrice.Count + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                foreach (var webElement in fixedPrice)
                {
                    Console.WriteLine(webElement.Text);
                }

                // IWebElement button = driver.FindElement(By.LinkText("Следующая страница"));
                // button.Click();
            
                Thread.Sleep(5000);

                
            }
            
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
}