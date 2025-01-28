using Microsoft.Win32;
using PROD_PdfJsonViewer_POC.UI.Helper;
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
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ContextFile _selectedPdfFile;
        private string _relatedJsonFile;
        private string _folderPath;

        private JsonDocument _jsonData;
        private Uri _pdfSource;

        private MainWindow _view;
        private ObservableCollection<JsonTreeNode> _jsonTree = new ObservableCollection<JsonTreeNode>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected PDF file.
        /// </summary>
        public ContextFile SelectedPdfFile
        {
            get { return _selectedPdfFile; }
            set { SetProperty(ref _selectedPdfFile, value); LoadPdfToViewer(); }
        }

        /// <summary>
        /// Gets or sets the related JSON file.
        /// </summary>
        public string RelatedJsonFile
        {
            get { return _relatedJsonFile; }
            set { SetProperty(ref _relatedJsonFile, value); }
        }

        /// <summary>
        /// Gets or sets the folder path.
        /// </summary>
        public string FolderPath
        {
            get { return _folderPath; }
            set { SetProperty(ref _folderPath, value); LoadLocalPdfFiles(); }
        }

        /// <summary>
        /// Gets or sets the JSON data.
        /// </summary>
        public JsonDocument JsonData
        {
            get { return _jsonData; }
            set { SetProperty(ref _jsonData, value); }
        }

        /// <summary>
        /// Gets or sets the JSON tree.
        /// </summary>
        public ObservableCollection<JsonTreeNode> JsonTree
        {
            get { return _jsonTree; }
            set { SetProperty(ref _jsonTree, value); }
        }

        /// <summary>
        /// Gets or sets the PDF source.
        /// </summary>
        public Uri PdfSource
        {
            get { return _pdfSource; }
            set { SetProperty(ref _pdfSource, value); }
        }

        /// <summary>
        /// Gets or sets the list of PDF files.
        /// </summary>
        public ObservableCollection<ContextFile> PdfFiles { get; set; } = new ObservableCollection<ContextFile>();

        #endregion

        #region Commands

        /// <summary>
        /// Command to open a PDF file.
        /// </summary>
        public ICommand OpenPdfFileCommand { get; }

        /// <summary>
        /// Command to load JSON data.
        /// </summary>
        public ICommand LoadJsonDataCommand { get; }

        /// <summary>
        /// Command to navigate to the previous PDF file.
        /// </summary>
        public ICommand PreviousPdfCommand { get; }

        /// <summary>
        /// Command to navigate to the next PDF file.
        /// </summary>
        public ICommand NextPdfCommand { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class with a view.
        /// </summary>
        /// <param name="view">The view.</param>
        public MainWindowViewModel(MainWindow view)
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            _view = view;
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Opens a PDF file.
        /// </summary>
        private void OpenPdfFile()
        {
            // Open file dialog and load selected PDF file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Select a PDF file"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedPdfFile = new ContextFile(openFileDialog.SafeFileName, openFileDialog.FileName);
                FolderPath = Path.GetDirectoryName(_selectedPdfFile.FilePath) ?? "File/Folder Not Found";
            }
        }

        /// <summary>
        /// Loads the PDF file to the viewer.
        /// </summary>
        private void LoadPdfToViewer()
        {
            // Find the related JSON file
            string jsonFile = GetRelatedJsonFile(_selectedPdfFile.FilePath);
            if (jsonFile != null)
            {
                // Load the JSON data and apply it to the TreeView
                RelatedJsonFile = jsonFile;
            }
            Debug.WriteLine(_selectedPdfFile.FilePath);
            _pdfSource = new Uri(_selectedPdfFile.FilePath);
            _view.PdfViewer.Navigate(_pdfSource);
        }

        /// <summary>
        /// Loads the local PDF files.
        /// </summary>
        private void LoadLocalPdfFiles()
        {
            // Load PDF files from local folder
            string[] pdfFiles = Directory.GetFiles(FolderPath, "*.pdf");
            PdfFiles.Clear();
            foreach (string pdfFile in pdfFiles)
            {
                PdfFiles.Add(new ContextFile(pdfFile));
            }
            var index = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile.FilePath);
            SelectedPdfFile = PdfFiles[index];
        }

        /// <summary>
        /// Loads the JSON data.
        /// </summary>
        private void LoadJsonData()
        {
            // Load the JSON data and apply it to the TreeView
            string jsonFile = GetRelatedJsonFile(_selectedPdfFile.FilePath);
            if (jsonFile != null)
            {
                LoadJsonData(jsonFile);
            }
        }

        /// <summary>
        /// Gets the related JSON file.
        /// </summary>
        /// <param name="pdfFile">The PDF file.</param>
        /// <returns>The related JSON file.</returns>
        private string GetRelatedJsonFile(string pdfFile)
        {
            // Find related JSON file
            string jsonFile = Path.ChangeExtension(pdfFile, ".json");
            return File.Exists(jsonFile) ? jsonFile : string.Empty;
        }

        /// <summary>
        /// Loads the JSON data from a file.
        /// </summary>
        /// <param name="jsonFile">The JSON file.</param>
        private void LoadJsonData(string jsonFile)
        {
            // Load JSON data
            string json = File.ReadAllText(jsonFile);
            JsonData = JsonDocument.Parse(json);
        }

        /// <summary>
        /// Adds a JSON node to the tree.
        /// </summary>
        /// <param name="jsonNode">The JSON node.</param>
        /// <param name="parentNode">The parent node.</param>
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

        /// <summary>
        /// Navigates to the previous PDF file.
        /// </summary>
        private void PreviousPdf()
        {
            int currentIndex = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile.FilePath);
            if (currentIndex > 0)
            {
                SelectedPdfFile = PdfFiles[currentIndex - 1];
            }
            else
            {
                SelectedPdfFile = PdfFiles[PdfFiles.Count - 1];
            }
        }

        /// <summary>
        /// Navigates to the next PDF file.
        /// </summary>
        private void NextPdf()
        {
            int currentIndex = PdfFiles.ToList().FindIndex(f => f.FilePath == SelectedPdfFile.FilePath);
            if (currentIndex < PdfFiles.Count - 1)
            {
                SelectedPdfFile = PdfFiles[currentIndex + 1];
            }
            else
            {
                SelectedPdfFile = PdfFiles[0];
            }
        }

        #endregion
    }
}
