using System.Text.Json;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using PROD_PdfJsonViewer_POC.UI.Helper;

namespace PROD_PdfJsonViewer_POC.UI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string _selectedPdfFile;
        private JsonDocument _jsonData;
        private TreeView treeView;

        public string SelectedPdfFile
        {
            get { return _selectedPdfFile; }
            set { SetProperty(ref _selectedPdfFile, value); }
        }

        public JsonDocument JsonData
        {
            get { return _jsonData; }
            set { SetProperty(ref _jsonData, value); }
        }

        public ICommand OpenPdfFileCommand { get; }
        public ICommand LoadJsonDataCommand { get; }

        public MainWindowViewModel()
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
        }

        private void OpenPdfFile()
        {
            // Open file dialog and load selected PDF file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.Title = "Select a PDF file";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedPdfFile = openFileDialog.FileName;
                // Find the related JSON file
                string jsonFile = GetRelatedJsonFile(_selectedPdfFile);
                if (jsonFile != null)
                {
                    // Load the JSON data and apply it to the TreeView
                    LoadJsonData(jsonFile);
                }
            }
        }

        private void LoadJsonData()
        {
            // Load JSON data from selected PDF file
        }

        private string GetRelatedJsonFile(string pdfFile)
        {
            // Find related JSON file
            string jsonFile = Path.ChangeExtension(pdfFile, ".json");
            if (File.Exists(jsonFile))
            {
                return jsonFile;
            }
            else
            {
                return null;
            }
        }

        private void LoadJsonData(string jsonFile)
        {
            // Load JSON data from file
            string json = File.ReadAllText(jsonFile);
            JsonDocument jsonData = JsonDocument.Parse(json);
            treeView.ItemsSource = jsonData.RootElement.GetProperty("data").EnumerateArray();
        }
    }
}
