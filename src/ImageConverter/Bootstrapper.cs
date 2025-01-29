using CommunityToolkit.Mvvm.DependencyInjection;
using ImageConverter.ViewModel;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

namespace ImageConverter
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            var services = ConfigureServices();
            Ioc.Default.ConfigureServices(services);
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <returns></returns>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services

            // WPF-UI Snackbar service 등록
            services.AddSingleton<ISnackbarService, SnackbarService>();

            // Viewer ViewModels
            services.AddTransient<ConverterViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
