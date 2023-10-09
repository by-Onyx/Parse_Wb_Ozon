using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using OfficeOpenXml;
using OpenQA.Selenium;

namespace ParseWbAndOzon;

public class TextWorker
{ 
    private List<ProductModel> products;
    private readonly string _fileDir;

    private List<string> elements = new ()
    {
        "Название;Бренд;Цена;Цена со скидкой;Рейтинг;Количество отзывов;Ссылка"
    };

    public TextWorker(List<ProductModel> products, string fileDir)
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

    public void WriteToExcelFile(string name)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        string filePath = Path.Combine(_fileDir, name + ".xlsx");
        FileInfo fileInfo = new FileInfo(filePath);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(name);
            
            string[] values = elements[0].Split(';');
                
            for (int columnIndex = 0; columnIndex < values.Length; columnIndex++)
            {
                worksheet.Cells[1, columnIndex + 1].Value = values[columnIndex];
            }
            
            for (int rowIndex = 1; rowIndex < elements.Count; rowIndex++)
            {
                values = elements[rowIndex].Split(';');
                
                for (int columnIndex = 0; columnIndex < values.Length - 1; columnIndex++)
                {
                    worksheet.Cells[rowIndex + 1, columnIndex + 1].Value = values[columnIndex];
                }
                
                worksheet.Cells[rowIndex + 1, 7].Formula = values[6];
            }
            
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.Protection.IsProtected = false;

            package.Save();
        }
    }
}