using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Windows;
using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;

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
                case "test":
                    OnTest();
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

        private void OnTest()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            
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
