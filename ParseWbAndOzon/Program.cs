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
        TextWorker textWorker = new TextWorker();
        FirefoxOptions options = new FirefoxOptions();
        //options.AddArgument("--headless");
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

                double speed = 0.5;
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
                
                textWorker.PrintInfo(productCards, textWorker);

                try
                {
                    IWebElement button = driver.FindElement(By.LinkText("Следующая страница"));
                    button.Click();
                }
                catch
                {
                    textWorker.WriteToFile("test");
                    break;
                }
            }
        }
        finally
        {
            driver.Quit();
        }
    }
}