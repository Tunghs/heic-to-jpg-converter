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
                        if (!ConvertFileList.Contains(file))
                            ConvertFileList.Add(file);
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
            // 다중 파일 선택 기능.m
            dlg.Multiselect = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (string file in dlg.FileNames)
                    if (!ConvertFileList.Contains(file))
                        ConvertFileList.Add(file);
            }
        }

        private void DeleteFile_OnClick()
        {
            foreach (string item in SelectedListViewItems)
            {
                ConvertFileList.Remove(item);
            }
        }


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

        private string _testText;
        public string TestText
        {
            get { return _testText; }
            set { _testText = value; RaisePropertyChanged("TestText"); }
        }


        //private CancellationTokenSource _CanceltokenCource;
        private void ConvertProcess_OnClick()
        {
            ((MetroWindow)Application.Current.MainWindow).ShowChildWindowAsync(new ProgressBarChildView() { DataContext = this });

            if(ConvertFileList.Count == 0)
            {
                List<string> fileList = new List<string>(ConvertFileList);
                ((MetroWindow)Application.Current.MainWindow).Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Ui update
                }));

                //CancellationToken cancelToken = _CanceltokenCource.Token;
                Task.Run(() =>
                {
                    try
                    {
                        double count = 1;
                        //foreach (string file in fileList)
                        //{
                        //    //RunConvertProcess(file);
                        //    double per = (count / (double)fileList.Count) * 100;

                        //    ProgerssBarChild.CurrentProgress = (int)per;
                        //}

                        //for (int i =0; i<10000; i++)
                        //{
                        //    double per = (count / (double)fileList.Count) * 100;


                        //    ((MetroWindow)Application.Current.MainWindow).Dispatcher.BeginInvoke(new Action(() =>
                        //    {
                        //        CurrentProgress = (int)per;
                        //    }));
                        //}
                        ((MetroWindow)Application.Current.MainWindow).Dispatcher.Invoke(new Action(() =>
                        {
                            TestText = "실행한다이";
                        }));
                        
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show("취소하셨습니다.");
                    }
                });
            }
            else
            {
                MessageBox.Show("No files to convert.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        #endregion
        #endregion

        public ProgressBarChildViewModel ProgerssBarChild { get; set; }

        public ConvertViewModel()
        {
            ProgerssBarChild = new ProgressBarChildViewModel();
            InitRelayCommand();
        }

        private void RunConvertProcess(string srcPath)
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
