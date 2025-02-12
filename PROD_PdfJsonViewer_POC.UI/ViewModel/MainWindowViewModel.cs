using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PROD_PdfJsonViewer_POC.UserControls.Models;
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
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContextFile _selectedPdfFile;

        [ObservableProperty]
        private ObservableCollection<ContextFile> _pdfFiles = new ObservableCollection<ContextFile>();

        [ObservableProperty]
        private string _relatedJsonFile;

        [ObservableProperty]
        private string _folderPath;

        [ObservableProperty]
        private JsonDocument _jsonData;

        [ObservableProperty]
        private Uri _pdfSource;

        [ObservableProperty]
        private ObservableCollection<JsonTreeNode> _jsonTree = new ObservableCollection<JsonTreeNode>();

        // Validation Panel Properties
        [ObservableProperty]
        private bool _validationIsExpanded = false;

        [ObservableProperty]
        private bool _validationIsPinned = false;

        private bool _validationFileExists = false;

        public ICommand OpenPdfFileCommand { get; }
        public ICommand LoadJsonDataCommand { get; }
        public ICommand PreviousPdfCommand { get; }
        public ICommand NextPdfCommand { get; }

        public MainWindowViewModel()
        {
            OpenPdfFileCommand = new RelayCommand(OpenPdfFile);
            LoadJsonDataCommand = new RelayCommand(LoadJsonData);
            PreviousPdfCommand = new RelayCommand(PreviousPdf);
            NextPdfCommand = new RelayCommand(NextPdf);
        }

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
                //SelectedPdfFile = new ContextFile(openFileDialog.SafeFileName, openFileDialog.FileName);
                FolderPath = Path.GetDirectoryName(openFileDialog.FileName) ?? "File/Folder Not Found";
                LoadLocalPdfFiles(openFileDialog.FileName);
                LoadPdfToViewer();
            }
        }

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

            Debug.WriteLine(PdfSource.ToString());
        }

        private void LoadLocalPdfFiles(string selectedFilePath)
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
            int index = PdfFiles.ToList().FindIndex(f => f.FilePath == selectedFilePath);
            if (index >= 0 && index < PdfFiles.Count)
                SelectedPdfFile = PdfFiles[index];
            else if (PdfFiles.Count > 0)
                SelectedPdfFile = PdfFiles[0];
        }

        private void LoadJsonData()
        {
            if (SelectedPdfFile == null || string.IsNullOrEmpty(SelectedPdfFile.FilePath))
                return;

            RelatedJsonFile = GetRelatedJsonFile(SelectedPdfFile.FilePath);
            
        }

        private string GetRelatedJsonFile(string pdfFile)
        {
            // Assume the related JSON file has the same name as the PDF file, but with a .json extension.
            string jsonFile = Path.ChangeExtension(pdfFile, ".json");
            return File.Exists(jsonFile) ? jsonFile : string.Empty;
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

        
        partial void OnSelectedPdfFileChanged(ContextFile value)
        {
            // TODO: Create Event handler for when the SelectedPdfFile property changes.
            LoadPdfToViewer();
            
        }
    }
}
