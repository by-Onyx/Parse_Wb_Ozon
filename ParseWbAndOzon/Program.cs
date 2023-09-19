﻿using System.Collections.ObjectModel;
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
        options.AddArgument("--headless");
        var driver = new FirefoxDriver(options);
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        try
        {
            driver.Navigate().GoToUrl("https://www.wildberries.ru/catalog/0/search.aspx?search=msi");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            driver.Manage().Window.Maximize();
            int y = 50;
            for (int timer = 0; timer < 19; timer++)
            {
                js.ExecuteScript($"window.scrollBy(0,{y.ToString()})");
                y += 50;
                Thread.Sleep(500);
            }
            

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            var price = driver.FindElements(By.TagName("article"));

            Console.Clear();
            Console.WriteLine(price.Count);
            foreach (var webElement in price)
            {
                Console.WriteLine(webElement.ToString());
            }

            IWebElement button = driver.FindElement(By.LinkText("Следующая страница"));
            button.Click();
            
            Thread.Sleep(10000);
        }
        finally
        {
            driver.Quit();
        }
    }
}