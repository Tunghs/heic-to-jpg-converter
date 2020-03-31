using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using ImageMagick;

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
                case "Convert":
                    OnConvertProcess();
                    break;
            }
        }

        private void OnConvertProcess()
        {
            if (File.Exists(SourcePath))
            {
                ConvertFile(SourcePath);
                MessageBox.Show("Convert 완료");
            }
            else if (Directory.Exists(SourcePath))
            {
                ConvertDirectory(SourcePath);
                MessageBox.Show("Convert 완료");
            }
            else
            {
                MessageBox.Show("경로를 다시 확인해주세요.");
            }     
        }
        #endregion
        #endregion

        public ConvertViewModel()
        {
            InitRelayCommand();
        }

        private void ConvertFile(string srcPath)
        {
            using (MagickImage img = new MagickImage(srcPath))
            {
                string saveDir = Path.Combine(Path.GetDirectoryName(srcPath), "SaveJPG");

                DirectoryInfo dir = new DirectoryInfo(saveDir);
                if (!dir.Exists)
                    dir.Create();

                string saveImgPath = Path.Combine(saveDir, Path.GetFileNameWithoutExtension(srcPath) + ".jpg");
                img.Write(saveImgPath);
            }
        }

        private void ConvertDirectory(string srcPath)
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension.ToLower() == ".heic")
                    ConvertFile(file.FullName);
            }
        }
    }
}
