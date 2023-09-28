using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ParseWbAndOzon.Parsers;

public abstract class Parser<T>
{
    public List<T> Products { get; private set; }
         
    protected WebDriver _driver;
    protected readonly string _productName;
    
    private const double scrollSpeed = 0.1;

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
        double currentPosition = 0;
        while (200 >= currentPosition)
        {
            currentPosition += scrollSpeed;
            _driver.ExecuteScript($"window.scrollBy(0,{currentPosition.ToString(CultureInfo.InvariantCulture)})");
        }
    }
}