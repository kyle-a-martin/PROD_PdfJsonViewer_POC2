using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Web.WebView2.Wpf;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROD_PdfJsonViewer_POC.UserControls.ViewModels
{
    public partial class PdfViewerViewModel : ObservableObject
    {
        private readonly IPdfViewerService _pdfViewerService;
        private WebView2 _webView;

        [ObservableProperty]
        private string filePath;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public PdfViewerViewModel(IPdfViewerService pdfViewerService)
        {
            _pdfViewerService = pdfViewerService;
        }

        public async Task InitializeWebView2Async(WebView2 webView)
        {
            _webView = webView;
            if (_webView?.CoreWebView2 != null)
                return;

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var env = await _pdfViewerService.InitializeWebView2EnvironmentAsync();
                await _webView.EnsureCoreWebView2Async(env);

                if (!string.IsNullOrEmpty(FilePath))
                {
                    await LoadPdfAsync();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to initialize WebView2: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadPdfAsync()
        {
            if (_webView?.CoreWebView2 == null)
                return;

            try
            {
                IsLoading = true;
                ErrorMessage = null;

                if (!_pdfViewerService.ValidatePdfPath(FilePath))
                {
                    ErrorMessage = $"PDF file not found: {FilePath}";
                    return;
                }

                var fileUri = _pdfViewerService.ConvertToFileUri(FilePath);
                _webView.CoreWebView2.Navigate(fileUri);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load PDF: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnFilePathChanged(string value)
        {
            LoadPdfCommand.NotifyCanExecuteChanged();
            _ = LoadPdfAsync();
        }

        [RelayCommand(CanExecute = nameof(CanLoadPdf))]
        private async Task LoadPdfAsync(bool forceReload = false)
        {
            await LoadPdfAsync();
        }

        private bool CanLoadPdf()
        {
            return !string.IsNullOrEmpty(FilePath) && !IsLoading;
        }
    }
}

