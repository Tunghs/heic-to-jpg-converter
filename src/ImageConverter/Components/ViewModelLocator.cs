using CommunityToolkit.Mvvm.DependencyInjection;

using ImageConverter.ViewModel;

namespace ImageConverter.Components
{
    public sealed class ViewModelLocator
    {
        public ConverterViewModel? ConverterViewModel
            => Ioc.Default.GetService<ConverterViewModel>();
    }
}
