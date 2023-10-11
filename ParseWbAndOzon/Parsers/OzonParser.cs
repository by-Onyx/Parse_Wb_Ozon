﻿using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace ParseWbAndOzon.Parsers;

public class OzonParser : Parser
{
    private int i = 0;
    private string productPricesCard;
    private int currentPageNumber;
    private Func<string> GetHandleLink;
    private string _normalLink;
    private int _pageNumberIndex;
    private bool _pastFirstPage = false; //На случай, если на первой странице вылетела капча
    public OzonParser(FirefoxDriver driver, FirefoxOptions options, string productName) : base(driver, options, productName)
    {
        currentPageNumber = 1;
        GetHandleLink = () =>
            handleLink =
                $"https://www.ozon.ru/search/?deny_category_prediction=true&from_global=true&page={currentPageNumber}&sorting=price_desc&text={productName}";
    }
    
    public override void Parse()
    {
        try
        {
            GetAllProductsCard();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            driver.Quit();
        }
    }

    protected override void NavigateToPage()
    {
        if (i == 0)
        {
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Navigate().GoToUrl(GetHandleLink());
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var button = driver.FindElement(By.CssSelector(".uy8 > button:nth-child(1)"));
            driver.Manage().Cookies.DeleteAllCookies();
            button.Click();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            if (handleLink.Contains("from_global"))
            {
                _normalLink = driver.Url.Replace("from_global=true", $"from_global=true&page={currentPageNumber}"); //TODO Засунуть newLink в Func GetHandleLink()
            }
            else
            {
                _normalLink = driver.Url.Replace("deny_category_prediction=true", $"deny_category_prediction=true&page={currentPageNumber}");
            }
            _pageNumberIndex = _normalLink.IndexOf('1', 50);
            i++;
            
            return;
        }

        _pastFirstPage = true;

        StringBuilder sb = new StringBuilder();
        sb.Append(_normalLink);
        sb.Remove(_pageNumberIndex, 1);
        sb.Insert(_pageNumberIndex, $"{currentPageNumber}");
        driver.Manage().Cookies.DeleteAllCookies();
        driver.Navigate().GoToUrl(sb.ToString());
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }
    
    protected override bool CheckNextPage()
    {
        try
        {
            driver.FindElement(By.LinkText("Дальше"));
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override void GetAllProductsCard()
    {
        do
        {
            NavigateToPage();

            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                var catalogCheck = driver.FindElement(By.CssSelector("#paginatorContent"));
            }
            catch(Exception e)
            {
                NewDriverConnection();
            }
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            var catalog = driver.FindElement(By.CssSelector("#paginatorContent"));
            if (!CheckBrandTag(catalog))
            {
                Console.WriteLine("fuck");
                currentPageNumber++;
                continue;
            }
            
            currentPageNumber++;
            
            ScrollToPageEnd(50);
            
            var productCardAttribute =
                catalog.FindElement(By.XPath("//*[@id=\"paginatorContent\"]/div/div/div[1]/div[1]"))
                    .GetAttribute("class")
                    .Replace(" ", ".");
            
            /*productPricesCard =
                catalog.FindElement(By.CssSelector($"#paginatorContent > div > div > div:nth-child(1) > div.{productCardAttribute} > div:nth-child(1) > div"))
                    .GetAttribute("class")
                    .Replace(" ", ".");*/
            
            //Products.AddRange(ProductToModel(catalog.FindElements(By.CssSelector($".{productCardAttribute}"))));

            GC.Collect();
        } while (CheckNextPage());
    }

    private void NewDriverConnection()
    {
        driver.Quit();
        driver = new FirefoxDriver(_options);
        driver.Manage().Cookies.DeleteAllCookies();
        if (!_pastFirstPage)
        {
            i = 0;
        }
        NavigateToPage();
    }
    
    protected override List<ProductModel> ProductToModel(ReadOnlyCollection<IWebElement> elements)
    {
        List<ProductModel> products = new();
        string price;
        string? priceWithSale = null;
        foreach (var element in elements)
        {
            var priceCard = element
                .FindElement(By.ClassName(productPricesCard)).Text;
            
            var priceInfo = priceCard.Split("\r\n");
            
            if (priceInfo.Length > 1)
            {
                priceWithSale = priceInfo[0];
                price = priceInfo[1];
            }
            else
            {
                price = priceInfo[0];
            }

            string? brand;
            try
            {
                brand = element.FindElement(By.ClassName("tsBody400Small")).Text;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            }
            catch
            {
                brand = null;
            }

            string? rating; 
            string? amountRewiew;
            
            try
            {
                var rewiews = element.FindElements(By.ClassName("v1"));
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
                rating = rewiews[0].Text.Trim();
                amountRewiew = rewiews[1].Text;
            }
            catch
            {
                rating = null;
                amountRewiew = null;
            }
            
            var product = new ProductModel
            {
                PriceWithSale = priceWithSale,
                Price = price,
                Brand = brand,
                Name = element.FindElement(By.ClassName("tsBody500Medium")).Text,
                Rating = rating,
                AmountRewiew = amountRewiew,
                Url = element.FindElement(By.TagName("a")).GetAttribute("href")
            };
            products.Add(product);
        }

        return products;
    }

    private bool CheckBrandTag(IWebElement catalog)
    {
        try
        {
            string brand = catalog
                .FindElement(By.ClassName("tsBody400Small"))
                .Text;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1);
            Console.WriteLine(brand);
            if (brand == "Стало дешевле") return false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}