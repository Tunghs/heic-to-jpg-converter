using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ImageConverter.Bases
{
    public abstract class ViewModelBase : ObservableValidator
    {
        public ViewModelBase() { }

        public virtual void Cleanup()
        {
            WeakReferenceMessenger.Default.Cleanup();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Cleanup();
            }
        }
    }
}
