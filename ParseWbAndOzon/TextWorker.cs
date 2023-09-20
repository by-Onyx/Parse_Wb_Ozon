using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using OpenQA.Selenium;

namespace ParseWbAndOzon;

public class TextWorker
{
    //private string title = "Имя;Бренд;Цена без скидки;Цена со скидкой;Рейтинг;Количество отзывов;Скидка";

    private List<string> elements = new List<string>()
    {
        "Имя;Бренд;Цена без скидки;Цена со скидкой;Рейтинг;Количество отзывов;Ссылка"
    };

    public void AddToList(Product product)
    {
        elements.Add(product.ToString());
    }

    public void WriteToFile(string name)
    {
        //elements.Add(title);
        string filePath = $"C:\\Users\\gosha\\RiderProjects\\Parse_Wb_Ozon\\{name}.csv";
        File.WriteAllLines(filePath, elements, Encoding.UTF8);
    }
    
    public void PrintInfo(ReadOnlyCollection<IWebElement> price, TextWorker textWorker)
    {
        int count = 0;
        foreach (var webElement in price)
        {
            var id = webElement.GetAttribute("id");
            var product = new Product
            {
                PriceWithSale = webElement
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > ins")).Text,
                Price = webElement
                    .FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > p > span > del")).Text,
                Brand =
                    webElement.FindElement(By.CssSelector($"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__brand")).Text,
                Name =
                    webElement.FindElement(
                            By.CssSelector(
                                $"#{id} > div > div.product-card__middle-wrap > h2 > span.product-card__name"))
                        .Text,
                Rating = webElement
                    .FindElement(By.CssSelector(
                        $"#{id
                        } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.address-rate-mini.address-rate-mini--sm"
                    ))
                    .Text,
                AmountRewiew =
                    webElement.FindElement(By.CssSelector($"#{id
                    } > div > div.product-card__bottom-wrap > p.product-card__rating-wrap > span.product-card__count"))
                        .Text,
                Url = $"https://www.wildberries.ru/catalog/{id.Replace("c", "")}/detail.aspx"
            };
            
            textWorker.AddToList(product);
        }
    }
}