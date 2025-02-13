using CommunityToolkit.Mvvm.ComponentModel;
using PROD_PdfJsonViewer_POC.UserControls.Enums;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    public partial class ContextFile : ObservableObject
    {
        [ObservableProperty]
        private string _fileName;

        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private bool _isValidated = false;

        [ObservableProperty]
        private DateTime? _validationDate;

        [ObservableProperty]
        private ValidationStatus _validationStatus = ValidationStatus.Pending;

        public ContextFile() 
        { 
            _fileName = string.Empty;
            _filePath = string.Empty;
        }

        public ContextFile(string filePath)
        {
            FilePath = filePath;
            FileName = System.IO.Path.GetFileName(filePath);
        }

        public ContextFile(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        partial void OnFilePathChanged(string value)
        {
            FileName = System.IO.Path.GetFileName(value);
        }

        public void Validate()
        {
            IsValidated = true;
            ValidationDate = DateTime.Now;
            ValidationStatus = ValidationStatus.Validated;
        }

        public void Reject()
        {
            IsValidated = false;
            ValidationDate = null;
            ValidationStatus = ValidationStatus.Rejected;
        }
    }
}
