using Microsoft.Win32;
using PROD_PdfJsonViewer_POC.UI.Helper;
using PROD_PdfJsonViewer_POC.UI.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Input;

namespace PROD_PdfJsonViewer_POC.UI.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private string _selectedPdfFile;
        private string _relatedJsonFile;
        private string _folderPath;

        private JsonDocument _jsonData;
        private Uri _pdfSource;

        private MainWindow _view;
        private ObservableCollection<JsonTreeNode> _jsonTree = new ObservableCollection<JsonTreeNode>();

        public ObservableCollection<string> PdfFiles { get; set; } = new ObservableCollection<string>(); // List of PDF files

        public ICommand PreviousPdfCommand { get; }

        public ICommand NextPdfCommand { get; }

        public string SelectedPdfFile
        {
            get { return _selectedPdfFile; }
            set { SetProperty(ref _selectedPdfFile, value); LoadPdfToViewer(); }
        }

        public string RelatedJsonFile
        {
            get { return _relatedJsonFile; }
            set { SetProperty(ref _relatedJsonFile, value); }
        }

        public string FolderPath
        {
            get { return _folderPath; }
            set { SetProperty(ref _folderPath, value); LoadLocalPdfFiles(); }
        }

        public JsonDocument JsonData
        {
            get { return _jsonData; }
            set { SetProperty(ref _jsonData, value); }
        }

        public ObservableCollection<JsonTreeNode> JsonTree
        {
            get { return _jsonTree; }
            set { SetProperty(ref _jsonTree, value); }
        }

        public Uri PdfSource
        {
            get { return _pdfSource; }
            set { SetProperty(ref _pdfSource, value); }
        }

        public ICommand OpenPdfFileCommand { get; }
        public ICommand LoadJsonDataCommand { get; }

        public MainWindowViewModel()
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

        public MainWindowViewModel(MainWindow view)
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            _view = view;
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

        private void OpenPdfFile()
        {
            // Open file dialog and load selected PDF file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            openFileDialog.Title = "Select a PDF file";
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedPdfFile = openFileDialog.FileName;
                FolderPath = Path.GetDirectoryName(_selectedPdfFile) ?? "File/Folder Not Found";
            }
        }

        private void LoadPdfToViewer()
        {
            // Find the related JSON file
            string jsonFile = GetRelatedJsonFile(_selectedPdfFile);
            if (jsonFile != null)
            {
                // Load the JSON data and apply it to the TreeView
                //LoadJsonData(jsonFile);
                RelatedJsonFile = jsonFile;
            }

            _pdfSource = new Uri(_selectedPdfFile);
            _view.PdfViewer.Navigate(_pdfSource);
        }

        private void LoadLocalPdfFiles()
        {

            // Load PDF files from local folder
            string[] pdfFiles = Directory.GetFiles(FolderPath, "*.pdf");
            PdfFiles.Clear();
            foreach (string pdfFile in pdfFiles)
            {
                PdfFiles.Add(pdfFile);
            }
        }

        private void LoadJsonData()
        {
            // Load the JSON data and apply it to the TreeView
            string jsonFile = GetRelatedJsonFile(_selectedPdfFile);
            if (jsonFile != null)
            {
                LoadJsonData(jsonFile);
            }
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
                return String.Empty;
            }
        }

        private void LoadJsonData(string jsonFile)
        {
            // Load JSON data
            string json = File.ReadAllText(jsonFile);
            JsonData = JsonDocument.Parse(json);
        }

        //private void LoadJsonData(string jsonFile)
        //{
        //    string json = File.ReadAllText(jsonFile);
        //    var jsonNode = JsonNode.Parse(json);

        //    JsonTree.Clear();

        //    if (jsonNode != null)
        //    {
        //        var root = new JsonTreeNode { };
        //        AddJsonNodeToTree(jsonNode, root);
        //        JsonTree.Add(root);
        //    }
        //}

        private void AddJsonNodeToTree(JsonNode jsonNode, JsonTreeNode parentNode)
        {
            switch (jsonNode)
            {
                case JsonObject jsonObject:
                    foreach (var property in jsonObject)
                    {
                        var childNode = new JsonTreeNode { Name = property.Key, IsEditable = true };
                        AddJsonNodeToTree(property.Value, childNode);
                        parentNode.Children.Add(childNode);
                    }
                    break;

                case JsonArray jsonArray:
                    for (int i = 0; i < jsonArray.Count; i++)
                    {
                        var childNode = new JsonTreeNode { Name = $"[{i}]", IsEditable = true };
                        AddJsonNodeToTree(jsonArray[i], childNode);
                        parentNode.Children.Add(childNode);
                    }
                    break;

                case JsonValue jsonValue:
                    parentNode.Value = jsonValue.ToString();
                    break;
            }
        }

        private void PreviousPdf()
        {
            int currentIndex = PdfFiles.IndexOf(SelectedPdfFile);
            if (currentIndex > 0)
            {
                SelectedPdfFile = PdfFiles[currentIndex - 1];
            }
            else
            {
                SelectedPdfFile = PdfFiles[PdfFiles.Count - 1];
            }
        }

        private void NextPdf()
        {
            int currentIndex = PdfFiles.IndexOf(SelectedPdfFile);
            if (currentIndex < PdfFiles.Count - 1)
            {
                SelectedPdfFile = PdfFiles[currentIndex + 1];
            }
            else
            {
                SelectedPdfFile = PdfFiles[0];
            }
        }
    }
}
