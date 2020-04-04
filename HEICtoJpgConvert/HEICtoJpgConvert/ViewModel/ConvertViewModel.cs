using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Windows;
using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;

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
        private ObservableCollection<string> _collectionFileList = new ObservableCollection<string>();
        public ObservableCollection<string> CollectionFileList
        {
            get { return _collectionFileList; }
            set { _collectionFileList = value; }
        }
        private string _selectedListBoxItem;
        public string SelectedListBoxItem
        {
            get { return _selectedListBoxItem; }
            set { _selectedListBoxItem = value; RaisePropertyChanged("SelectedListBoxItem"); }
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
                case "UploadFilesClick":
                    UploadFilesClick();
                    break;
                case "DeleteFileClick":
                    DeleteFileClick();
                    break;
            }
        }

        /// <summary>
        /// UploadFiles Button Click
        /// </summary>
        private void UploadFilesClick()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();

            dlg.Title = "Select the HEIC Files";
            dlg.InitialDirectory = "C:\\";
            dlg.Filters.Add(new CommonFileDialogFilter("HEIC Files", "*.heic"));
            // 다중 파일 선택 기능.
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string file in dlg.FileNames)
                    CollectionFileList.Add(file);
            }
        }

        private void DeleteFileClick()
        {
            MessageBox.Show(SelectedListBoxItem);
            // CollectionFileList.Remove(SelectedListBoxItem);
        }

        /// <summary>
        /// 코드 수정 후 변경!
        /// </summary>
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
