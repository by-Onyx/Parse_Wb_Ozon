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
        /*FirefoxOptions options = new FirefoxOptions();
        options.AddArgument("--headless");
        var driver = new FirefoxDriver(options);
        WbParser wb = new WbParser(driver, "msi+материнская+плата");
        wb.Parse();
        TextWorker<Product> textWorker = new TextWorker<Product>(wb.Products);
        textWorker.WriteToFile("we");*/
    }
}