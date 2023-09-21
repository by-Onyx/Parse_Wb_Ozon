using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using OpenQA.Selenium;

namespace ParseWbAndOzon;

public class TextWorker<T>
{ 
    private IReadOnlyCollection<T> products;
    private readonly string _fileDir;
    
    private List<string> elements = new()
    {
        "Имя;Бренд;Цена без скидки;Цена со скидкой;Рейтинг;Количество отзывов;Ссылка"
    };

    public TextWorker(IReadOnlyCollection<T> products, string fileDir)
    {
        this.products = products;

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