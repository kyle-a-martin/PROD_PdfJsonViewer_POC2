using Microsoft.Web.WebView2.Core;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UserControls.Services.Implementations
{
    public class PdfViewerService : IPdfViewerService
    {
        public string ConvertToFileUri(string filePath)
        {
            return new Uri(filePath).AbsoluteUri;
        }

        public async Task<CoreWebView2Environment> InitializeWebView2EnvironmentAsync()
        {
            var cachePath = Path.Combine(Path.GetTempPath(), "WebView2Cache");
            var options = new CoreWebView2EnvironmentOptions("--disable-pdf-new-viewer=1");

            return await CoreWebView2Environment.CreateAsync(null, cachePath, options);
        }

        public bool ValidatePdfPath(string filePath)
        {
            return !string.IsNullOrEmpty(filePath) && File.Exists(filePath);
        }
    }
}
