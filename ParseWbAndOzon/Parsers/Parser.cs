using System.Collections.ObjectModel;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace ParseWbAndOzon.Parsers;

public abstract class Parser
{
    public List<ProductModel> Products { get; private set; }
         
    protected FirefoxDriver driver;
    protected string handleLink;
    protected int currentPageNumber;
    protected Func<string> GetHandleLink;
    
    protected readonly string _productName;
    protected readonly string _brand;
    protected readonly FirefoxOptions _options;
    
    private const double _scrollSpeed = 0.1;
    
    public Parser(FirefoxDriver driver, FirefoxOptions options, string productName)
    {
        this.driver = driver;
        _options = options;
        _productName = productName;
        Products = new List<ProductModel>();
        currentPageNumber = 1;
        _brand = productName.Split('+')[0];
    }

    public abstract void Parse();
    
    protected abstract bool CheckNextPage();
    protected abstract void NavigateToPage();
    protected abstract void GetAllProductsCard();
    protected abstract List<ProductModel> ProductToModel(ReadOnlyCollection<IWebElement> elements);
    
    protected  void ScrollToPageEnd(int pageSize)
    {
        double currentPosition = 0;
        while (pageSize >= currentPosition)
        {
            currentPosition += _scrollSpeed;
            driver.ExecuteScript($"window.scrollBy(0,{currentPosition.ToString(CultureInfo.InvariantCulture)})");
        }
    }

    protected abstract string GetBrandId();
}