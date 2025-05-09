using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces
{
    public interface IPdfViewerService
    {
        Task<CoreWebView2Environment> InitializeWebView2EnvironmentAsync();
        bool ValidatePdfPath(string filePath);
        string ConvertToFileUri(string filePath);
    }
}
