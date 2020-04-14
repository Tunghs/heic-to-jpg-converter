using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using ImageMagick;

namespace HEICtoJpgConvert.ViewModel
{
    public class ConvertViewModel : ViewModelBase
    {
        #region UIVariable
        private string _saveDirPath;
        public string SaveDirPath
        {
            get { return _saveDirPath; }
            set { _saveDirPath = value; RaisePropertyChanged("SaveDirPath"); }
        }
        private ObservableCollection<string> _collectionFileList = new ObservableCollection<string>();
        public ObservableCollection<string> CollectionFileList
        {
            get { return _collectionFileList; }
            set { _collectionFileList = value; }
        }
        private bool _saveDirCheck = false;
        public bool SaveDirCheck
        {
            get { return _saveDirCheck; }
            set { _saveDirCheck = value; RaisePropertyChanged("SaveDirCheck"); }
        }
        private bool _saveDirControlEnabled = false;
        public bool SaveDirControlEnabled
        {
            get { return _saveDirControlEnabled; }
            set { _saveDirControlEnabled = value; RaisePropertyChanged("SaveDirControlEnabled"); }
        }
        private List<string> _selectedListViewItems = new List<string>();
        public List<string> SelectedListViewItems
        {
            get { return _selectedListViewItems; }
            set { _selectedListViewItems = value; RaisePropertyChanged("SelectedListViewItems"); }
        }
        #endregion

        #region Command
        public RelayCommand<object> ButtonClickCommand { get; private set; }
        public RelayCommand<object> CheckBoxClickCommand { get; private set; }
        public RelayCommand<IList> ListViewSelectCommand { get; private set; }

        private void InitRelayCommand()
        {
            ButtonClickCommand = new RelayCommand<object>(OnButtonClick);
            CheckBoxClickCommand = new RelayCommand<object>(CheckBox_OnClick);
            ListViewSelectCommand = new RelayCommand<IList>(ListViewSelect);
        }

        #region CommandAction
        private void ListViewSelect(IList param)
        {
            SelectedListViewItems = param.Cast<string>().ToList();
        }

        private void CheckBox_OnClick(object param)
        {
            bool isCheck;

            isCheck = (!SaveDirCheck) ? true : false;

            SaveDirCheck = isCheck;
            SaveDirControlEnabled = isCheck;

            //if (!SaveDirCheck)
        }
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
            // MessageBox.Show(SaveDirControlEnabled.ToString());
            MessageBox.Show(SelectedListViewItems[0].ToString());
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
            string saveDir = Path.Combine(Path.GetDirectoryName(srcPath), "SaveJPG");

            if (SaveDirCheck)
                saveDir = SaveDirPath;

            DirectoryInfo dir = new DirectoryInfo(saveDir);
            if (!dir.Exists)
                dir.Create();

            using (MagickImage img = new MagickImage(srcPath))
            {
                string saveImgPath = Path.Combine(saveDir, Path.GetFileNameWithoutExtension(srcPath) + ".jpg");
                img.Write(saveImgPath);
            }
        }
    }
}
