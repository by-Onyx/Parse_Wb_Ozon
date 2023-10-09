using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using ParseWbAndOzon;
using ParseWbAndOzon.Parsers;
using ParseWbAndOzon.Parsers;

namespace ParseUI
{
    public partial class MainWindow : Window
    {
        private string fileDir;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtSearchPlaceholder.Visibility = txtSearch.Text != "" ? Visibility.Hidden : Visibility.Visible;
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowFolderDialog();
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--headless");
            var driver = new FirefoxDriver(options);

            if ((bool)wildberriesCheck.IsChecked)
            {
                WbParser wb = new WbParser(driver, txtSearch.Text.Replace(" ", "+"));
                wb.Parse();
                TextWorker textWorker = new TextWorker(wb.Products, fileDir);
                textWorker.WriteToExcelFile($"wb_{txtSearch.Text.Replace(" ", "_")}_" +
                                            $"{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}");
            }
            if ((bool)ozonCheck.IsChecked)
            {
                OzonParser ozon = new OzonParser(driver, options,txtSearch.Text.Replace(" ", "+"));
                ozon.Parse();
                TextWorker textWorker = new TextWorker(ozon.Products, fileDir);
                textWorker.WriteToExcelFile($"ozon_{txtSearch.Text.Replace(" ", "_")}_" +
                                       $"{DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")}");
            }
        }

        private void ShowFolderDialog()
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog();
            folderDialog.IsFolderPicker = true;
            folderDialog.Title = "Выберите папку для сохранения файла";

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                fileDir = folderDialog.FileName;
            }
        }
    }
}