using Microsoft.Extensions.DependencyInjection;
using PROD_PdfJsonViewer_POC.UserControls.Services.Interfaces;
using PROD_PdfJsonViewer_POC.UserControls.Services.Implementations;
using PROD_PdfJsonViewer_POC.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PROD_PdfJsonViewer_POC.UserControls
{
    public static class DIRegistration
    {
        public static IServiceCollection AddJsonEditorControlLibrary(this IServiceCollection services)
        {
            services.AddTransient<IJsonFileService, JsonFileService>();
            services.AddTransient<JsonEditorViewModel>();
            services.AddLogging(configure => configure.AddConsole());
            return services;
        }
    }
}
