using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HEICtoJpgConvert.ViewModel
{
    public class ConvertViewModel : ViewModelBase
    {
        #region UIVariable
        private string _sourcePath;
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; RaisePropertyChanged("SourcePath"); }
        }
        private string _savePath;
        public string SavePath
        {
            get { return _savePath; }
            set { _savePath = value; RaisePropertyChanged("SavePath"); }
        }
        #endregion

        #region Command
        public RelayCommand<object> ButtonClickCommand { get; private set; }

        private void InitRelayCommand()
        {
            ButtonClickCommand = new RelayCommand<object>((param) => OnButtonClick(param));
        }

        #region CommandAction
        private void OnButtonClick(object param)
        {
            switch (param.ToString())
            {
                case "OpenFileBrowser":
                    OnConvertProcess();
                    break;
            }
        }

        private void OnConvertProcess()
        {
            if ()
        }
        #endregion
        #endregion
    }
}
