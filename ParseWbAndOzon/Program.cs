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
        FirefoxOptions options = new FirefoxOptions();
        var driver = new FirefoxDriver(options);
        try {
            driver.Navigate().GoToUrl("https://www.wildberries.ru/catalog/0/search.aspx?search=msi");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            /*driver.ExecuteScript(
                "window.scrollTo(0,document.body.scrollHeight)");

            Thread.Sleep(3000);*/
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            var price = driver.FindElements(By.TagName("article"));
            
            Console.WriteLine(price.Count);
            foreach (var webElement in price)
            {
                Console.WriteLine(webElement.Text);
            }
        } finally {
            driver.Quit();
        }
    } 
}
