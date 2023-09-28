using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace ParseWbAndOzon.Parsers;

public class OzonParser : Parser<OzonProduct>
{
    public OzonParser(WebDriver driver, string productName) : base(driver, productName) { }

    public override void Parse()
    {
        try
        {
            NavigateToPage();
            GetAllProductsCard();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _driver.Quit();
        }
    }
    
    protected override void NavigateToPage()
    {
        _driver.Navigate().GoToUrl($"https://www.ozon.ru/search?text={_productName}");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    protected override bool CheckNextPage()
    {
        throw new NotImplementedException();
    }

    protected override void GetAllProductsCard()
    {
        throw new NotImplementedException();
    }

    protected override List<OzonProduct> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        throw new NotImplementedException();
    }
}