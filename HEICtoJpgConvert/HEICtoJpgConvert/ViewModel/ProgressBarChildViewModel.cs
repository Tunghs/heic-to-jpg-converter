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
        private string _saveDirPath = "hiiiiiiiiiiiiiiiiiii";
        public string SaveDirPath
        {
            get { return _saveDirPath; }
            set { _saveDirPath = value; RaisePropertyChanged("SaveDirPath"); }
        }

        public ProgressBarChildViewModel()
        {

        }
    }
}
