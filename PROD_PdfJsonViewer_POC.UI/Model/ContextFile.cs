namespace PROD_PdfJsonViewer_POC.UI.Model
{
    public class ContextFile
    {
        private string _fileName;
        public string FileName { get => _fileName; set => _fileName = value; }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                SetFileName(value);
            }
        }

        public ContextFile(string filePath)
        {
            _filePath = filePath;
            _fileName = FindFileName(filePath);
        }

        public ContextFile(string fileName, string filePath)
        {
            _fileName = fileName;
            _filePath = filePath;
        }

        private string FindFileName(string filePath)
        {
            return System.IO.Path.GetFileName(filePath);
        }

        private void SetFileName(string filePath)
        {
            _fileName = System.IO.Path.GetFileName(filePath);
        }
    }
}
