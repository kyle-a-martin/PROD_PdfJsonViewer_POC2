using System.Text.Json;
using System.Windows.Input;
using PROD_PdfJsonViewer_POC.UI.Helper;

namespace PROD_PdfJsonViewer_POC.UI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string _selectedPdfFile;
        private JsonDocument _jsonData;

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
        }

        private void LoadJsonData()
        {
            // Load JSON data from selected PDF file
        }

        private string GetRelatedJsonFile(string pdfFile)
        {
            // Find related JSON file
        }

        private JsonDocument LoadJsonData(string jsonFile)
        {
            // Load JSON data from file
        }
    }
}
