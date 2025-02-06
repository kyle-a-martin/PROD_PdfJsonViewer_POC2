using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PROD_PdfJsonViewer_POC.UI.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Input;

namespace PROD_PdfJsonViewer_POC.UI.ViewModel
{
    /// <summary>
    /// View model for the main window.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        private ContextFile _selectedPdfFile;
        private string _relatedJsonFile;
        private string _folderPath;
        private JsonDocument _jsonData;
        private Uri _pdfSource;
        private ObservableCollection<JsonTreeNode> _jsonTree = new ObservableCollection<JsonTreeNode>();

        #region Properties

        public ContextFile SelectedPdfFile
        {
            get => _selectedPdfFile;
            set
            {
                if (SetProperty(ref _selectedPdfFile, value))
                {
                    LoadPdfToViewer();
                }
            }
        }

        public string RelatedJsonFile
        {
            get => _relatedJsonFile;
            set => SetProperty(ref _relatedJsonFile, value);
        }

        public string FolderPath
        {
            get => _folderPath;
            set
            {
                if (SetProperty(ref _folderPath, value))
                {
                    LoadLocalPdfFiles();
                }
            }
        }

        public JsonDocument JsonData
        {
            get => _jsonData;
            set => SetProperty(ref _jsonData, value);
        }

        public ObservableCollection<JsonTreeNode> JsonTree
        {
            get => _jsonTree;
            set => SetProperty(ref _jsonTree, value);
        }

        public Uri PdfSource
        {
            get => _pdfSource;
            set => SetProperty(ref _pdfSource, value);
        }

        public ObservableCollection<ContextFile> PdfFiles { get; set; } = new ObservableCollection<ContextFile>();

        #endregion

        #region Commands

        public ICommand OpenPdfFileCommand { get; }
        public ICommand LoadJsonDataCommand { get; }
        public ICommand PreviousPdfCommand { get; }
        public ICommand NextPdfCommand { get; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

        #endregion

        #region Methods

        private void OpenPdfFile()
        {
            // Open a file dialog to select a PDF file.
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select a PDF file"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedPdfFile = new ContextFile(openFileDialog.SafeFileName, openFileDialog.FileName);
                FolderPath = Path.GetDirectoryName(SelectedPdfFile.FilePath) ?? "File/Folder Not Found";
            }
        }

        /// <summary>
        /// Updates the PDF source and finds the related JSON file.
        /// The view will subscribe to changes in PdfSource and navigate the WebBrowser accordingly.
        /// </summary>
        private void LoadPdfToViewer()
        {
            if (SelectedPdfFile == null || string.IsNullOrEmpty(SelectedPdfFile.FilePath))
                return;

            // Find the related JSON file.
            string jsonFile = GetRelatedJsonFile(SelectedPdfFile.FilePath);
            if (!string.IsNullOrEmpty(jsonFile))
            {
                RelatedJsonFile = jsonFile;
            }
            Debug.WriteLine(SelectedPdfFile.FilePath);

            // Update the PdfSource property.
            PdfSource = new Uri(SelectedPdfFile.FilePath);
        }

        private void LoadLocalPdfFiles()
        {
            if (string.IsNullOrEmpty(FolderPath) || !Directory.Exists(FolderPath))
                return;

            string[] pdfFiles = Directory.GetFiles(FolderPath, "*.pdf");
            PdfFiles.Clear();
            foreach (string pdfFile in pdfFiles)
            {
                PdfFiles.Add(new ContextFile(pdfFile));
            }

            // Select the matching PDF file if it exists, or default to the first file.
            int index = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile?.FilePath);
            if (index >= 0 && index < PdfFiles.Count)
                SelectedPdfFile = PdfFiles[index];
            else if (PdfFiles.Count > 0)
                SelectedPdfFile = PdfFiles[0];
        }

        private void LoadJsonData()
        {
            if (SelectedPdfFile == null || string.IsNullOrEmpty(SelectedPdfFile.FilePath))
                return;

            string jsonFile = GetRelatedJsonFile(SelectedPdfFile.FilePath);
            if (!string.IsNullOrEmpty(jsonFile))
            {
                LoadJsonData(jsonFile);
            }
        }

        private string GetRelatedJsonFile(string pdfFile)
        {
            // Assume the related JSON file has the same name as the PDF file, but with a .json extension.
            string jsonFile = Path.ChangeExtension(pdfFile, ".json");
            return File.Exists(jsonFile) ? jsonFile : string.Empty;
        }

        private void LoadJsonData(string jsonFile)
        {
            string json = File.ReadAllText(jsonFile);
            JsonData = JsonDocument.Parse(json);
        }

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
            int currentIndex = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile.FilePath);
            if (currentIndex > 0)
            {
                SelectedPdfFile = PdfFiles[currentIndex - 1];
            }
            else if (PdfFiles.Count > 0)
            {
                SelectedPdfFile = PdfFiles[PdfFiles.Count - 1];
            }
        }

        private void NextPdf()
        {
            int currentIndex = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile.FilePath);
            if (currentIndex < PdfFiles.Count - 1)
            {
                SelectedPdfFile = PdfFiles[currentIndex + 1];
            }
            else if (PdfFiles.Count > 0)
            {
                SelectedPdfFile = PdfFiles[0];
            }
        }

        #endregion
    }
}
