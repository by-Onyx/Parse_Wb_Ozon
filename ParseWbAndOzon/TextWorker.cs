using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using OpenQA.Selenium;

namespace ParseWbAndOzon;

public class TextWorker<T>
{ 
    private IReadOnlyCollection<T> products;
    private readonly string _fileDir;

    private List<string> elements;

    public TextWorker(IReadOnlyCollection<T> products, string fileDir)
    {
        this.products = products;
        if (typeof(T) == typeof(OzonProduct))
        {
            elements = new List<string>()
            {
                "Название;Цена;Цена со скидкой;Размер скидки;Ссылка"
            };
        }
        else if (typeof(T) == typeof(ProductModel))
        {
            elements = new List<string>()
            {
                "Название;Бренд;Цена;Цена со скидкой;Рейтинг;Количество отзывов;Ссылка"
            };
        }

        AddToList();
        _fileDir = fileDir;
    }

    public void AddToList()
    {
        foreach (var product in products)
        {
            elements.Add(product.ToString());
        }
    }
    
    public void WriteToFile(string name)
    {
        
        string filePath = $"{_fileDir}\\{name}.csv";
        File.WriteAllLines(filePath, elements, Encoding.UTF8);
    }
}