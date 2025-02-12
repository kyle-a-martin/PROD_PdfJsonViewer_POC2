using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    public class ValidationReport
    {
        public required string DirectoryPath { get; set; }
        public required List<ContextFile> Files { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
