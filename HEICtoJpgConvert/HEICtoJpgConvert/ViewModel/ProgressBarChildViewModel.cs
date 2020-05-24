using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEICtoJpgConvert.ViewModel
{
    public class ProgressBarChildViewModel : ViewModelBase
    {
        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    RaisePropertyChanged("CurrentProgress");
                }
            }
        }
        public ProgressBarChildViewModel()
        {

        }
    }
}
