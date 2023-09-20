using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ParseWbAndOzon.Parsers;

public abstract class Parser<T>
{
    public List<T> Products { get; set; }
         
    protected readonly WebDriver _driver;
    protected readonly string _productName;
    
    private const double speed = 0.5;

    public Parser(WebDriver driver, string productName)
    {
        this._driver = driver;
        this._productName = productName;
        this.Products = new();
    }

    public abstract void Parse();
    
    protected abstract bool CheckNextPage();
    protected abstract void NavigateToPage();
    protected abstract void GetAllProductsCard();
    protected abstract List<T> ProductToModel(ReadOnlyCollection<IWebElement> elements);
    
    protected void ScrollToPageEnd()
    {
        int pageHeight = Convert.ToInt32(_driver.ExecuteScript("return document.body.scrollHeight"));
        double position = 0;
        while (pageHeight - 200 > position)
        {
            position += speed;
            _driver.ExecuteScript($"window.scrollBy(0,{position.ToString(CultureInfo.InvariantCulture)})");
        }
    }
}