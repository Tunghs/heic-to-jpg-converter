using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HEICtoJpgConvert.View;
using ImageMagick;
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        private ObservableCollection<string> _converFileList = new ObservableCollection<string>();
        public ObservableCollection<string> ConvertFileList
        {
            get { return _converFileList; }
            set { _converFileList = value; }
        }
        private List<string> _selectedListViewItems = new List<string>();
        public List<string> SelectedListViewItems
        {
            get { return _selectedListViewItems; }
            set { _selectedListViewItems = value; RaisePropertyChanged("SelectedListViewItems"); }
        }

        private bool _isSaveDirChecked = false;
        public bool IsSaveDirChecked
        {
            get { return _isSaveDirChecked; }
            set { _isSaveDirChecked = value; RaisePropertyChanged("IsSaveDirChecked"); }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; RaisePropertyChanged("IsChecked"); }
        }

        private Brush _listViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
        public Brush ListViewBorderColor
        {
            get { return _listViewBorderColor; }
            set { _listViewBorderColor = value; RaisePropertyChanged("ListViewBorderColor"); }
        }

        private int _progressMaximum;
        public int ProgressMaximum
        {
            get { return _progressMaximum; }
            set { _progressMaximum = value; RaisePropertyChanged("ProgressMaximum"); }
        }

        private int _progressValue;
        public int ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; RaisePropertyChanged("ProgressValue"); }
        }

        private string _convertingFileName;
        public string ConvertingFileName
        {
            get { return _convertingFileName; }
            set { _convertingFileName = value; RaisePropertyChanged("ConvertingFileName"); }
        }
        #endregion

        #region Command
        public RelayCommand<object> ButtonClickCommand { get; private set; }
        public RelayCommand<object> CheckBoxClickCommand { get; private set; }
        public RelayCommand<IList> ListViewSelectCommand { get; private set; }
        public RelayCommand<DragEventArgs> ListViewDropCommand { get; private set; }
        public RelayCommand<DragEventArgs> ListViewDragOverCommand { get; private set; }
        public RelayCommand<DragEventArgs> ListViewDragLeaveCommand { get; private set; }

        private void InitRelayCommand()
        {
            ButtonClickCommand = new RelayCommand<object>(OnButtonClick);
            CheckBoxClickCommand = new RelayCommand<object>(CheckBox_OnClick);
            ListViewSelectCommand = new RelayCommand<IList>(ReceiveListViewSelectItems);
            ListViewDropCommand = new RelayCommand<DragEventArgs>(OnListViewDrop);
            ListViewDragOverCommand = new RelayCommand<DragEventArgs>(OnListViewDragOver);
            ListViewDragLeaveCommand = new RelayCommand<DragEventArgs>(OnListViewDragLeave);
        }

        #region CommandAction

        // Receive multiple items of list view
        private void ReceiveListViewSelectItems(IList param)
        {
            SelectedListViewItems = param.Cast<string>().ToList();
        }

        private void OnListViewDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (file.ToLower().Contains("heic"))
                    {
                        if (!ConvertFileList.Contains(file))
                        {
                            ConvertFileList.Add(file);
                        }
                        else
                        {
                            MessageBox.Show("이미 리스트에 포함되어있습니다.");
                        }   
                    }   
                }
                ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
            }
        }

        private void OnListViewDragOver(DragEventArgs e)
        {
            e.Handled = true;
            ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("Black"));
        }

        private void OnListViewDragLeave(DragEventArgs e)
        {
            ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
        }

        private void CheckBox_OnClick(object param)
        {
            bool isCheck;
            isCheck = (!IsSaveDirChecked) ? true : false;
            IsSaveDirChecked = isCheck;
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
                    Convert_OnClick();
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
            // 다중 파일 선택 기능.m
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string file in dlg.FileNames)
                {
                    if (!ConvertFileList.Contains(file))
                    {
                        ConvertFileList.Add(file);
                    }
                    else
                    {
                        MessageBox.Show("이미 리스트에 포함되어있습니다.");
                    }
                }
            }
        }

        private void DeleteFile_OnClick()
        {
            foreach (string item in SelectedListViewItems)
            {
                ConvertFileList.Remove(item);
            }
        }

        //private CancellationTokenSource _CanceltokenCource;
        private void Convert_OnClick()
        {
            if(ConvertFileList.Count != 0)
            {
                
                RunConvertProcess();
            }
            else
            {
                MessageBox.Show("No files to convert.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion
        #endregion

        public ConvertViewModel()
        {
            InitRelayCommand();
        }

        private void RunConvertProcess()
        {
            ((MetroWindow)Application.Current.MainWindow).ShowChildWindowAsync(new ProgressBarChildView() { DataContext = this });
            List<string> fileList = new List<string>(ConvertFileList);

            //CancellationToken cancelToken = _CanceltokenCource.Token;
            Task.Run(() =>
            {
                try
                {
                    ProgressMaximum = fileList.Count;

                    int count = 1;
                    foreach (string file in fileList)
                    {
                        ConvertingFileName = file;
                        ConvertingHeicToJpg(file);
                        ProgressValue += count;
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("취소하셨습니다.");
                }
            });
        }

        private void ConvertingHeicToJpg(string srcPath)
        {
            string saveDir = Path.Combine(Path.GetDirectoryName(srcPath), "SaveJPG");

            if (IsSaveDirChecked)
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
