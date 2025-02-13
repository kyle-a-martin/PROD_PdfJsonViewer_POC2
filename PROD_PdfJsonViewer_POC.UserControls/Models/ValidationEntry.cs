using PROD_PdfJsonViewer_POC.UserControls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UserControls.Models
{
    public class ValidationEntry
    {
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public ValidationStatus Status { get; set; }
        public DateTime? ValidationDate { get; set; }
    }
}
