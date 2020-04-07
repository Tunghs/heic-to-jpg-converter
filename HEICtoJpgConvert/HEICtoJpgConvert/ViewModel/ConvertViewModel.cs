using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Windows;
using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
        //private  _selectedListBoxItems = new List<string>();
        //public List<string> SelectedListBoxItems
        //{
        //    get { return _selectedListBoxItems; }
        //    set { _selectedListBoxItems = value; RaisePropertyChanged("SelectedListBoxItems"); }
        //}
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
                    UploadFiles_OnClick();
                    break;
                case "DeleteFileClick":
                    DeleteFile_OnClick();
                    break;
                case "Convert":
                    ConvertProcess_OnClick();
                    break;
            }
        }

        /// <summary>
        /// UploadFiles Button Click
        /// </summary>
        private void UploadFiles_OnClick()
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

        private void DeleteFile_OnClick()
        {
            MessageBox.Show("클릭");
        }

        /// <summary>
        /// 코드 수정 후 변경!
        /// </summary>
        private void ConvertProcess_OnClick()
        {
            if (CollectionFileList.Count != 0)
            {
                foreach (string filePath in CollectionFileList)
                    ConvertProcess(filePath);

                MessageBox.Show("Convert Success!");
            }

            MessageBox.Show("No files to convert.", "Warning",MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion       
        #endregion

        public ConvertViewModel()
        {
            InitRelayCommand();
        }

        private void ConvertProcess(string srcPath)
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
    }
}
