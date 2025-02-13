using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PROD_PdfJsonViewer_POC.UI.ViewModel;
using PROD_PdfJsonViewer_POC.UserControls.Services.Implementations;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System;
using System.Windows;

namespace PROD_PdfJsonViewer_POC.UI
{
    public partial class App : Application
    {
        public IHost AppHost { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .Build();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            await AppHost.StartAsync();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register application services
            services.AddSingleton<IJsonFileService, JsonFileService>();
            services.AddSingleton<ILogger<ValidationPanelViewModel>, Logger<ValidationPanelViewModel>>();

            // Register ViewModels
            services.AddSingleton<ValidationPanelViewModel>();
            services.AddTransient<MainWindowViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (AppHost != null)
            {
                await AppHost.StopAsync(TimeSpan.FromSeconds(5));
                AppHost.Dispose();
            }
            base.OnExit(e);
        }
    }
}