using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PROD_PdfJsonViewer_POC.UserControls;
using Microsoft.Extensions.Logging;
using System.Windows;
using PROD_PdfJsonViewer_POC.UI.ViewModel;

namespace PROD_PdfJsonViewer_POC.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();


                })
                .Build();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            await _host.StartAsync();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
                _host.Dispose();
            }
            base.OnExit(e);
        }
    }

}
