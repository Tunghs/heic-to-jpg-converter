using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using ImageMagick;
using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using MahApps.Metro.SimpleChildWindow;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using HEICtoJpgConvert.View;

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
        private Brush _listViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
        public Brush ListViewBorderColor
        {
            get { return _listViewBorderColor; }
            set { _listViewBorderColor = value; RaisePropertyChanged("ListViewBorderColor"); }
        }
        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    RaisePropertyChanged("CurrentProgress");
                }
            }
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
            ListViewSelectCommand = new RelayCommand<IList>(ListViewSelect);
            ListViewDropCommand = new RelayCommand<DragEventArgs>(OnListViewDrop);
            ListViewDragOverCommand = new RelayCommand<DragEventArgs>(OnListViewDragOver);
            ListViewDragLeaveCommand = new RelayCommand<DragEventArgs>(OnListViewDragLeave);
        }

        #region CommandAction
        private void ListViewSelect(IList param)
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
                        if(!CollectionFileList.Contains(file))
                            CollectionFileList.Add(file);
                }
                ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
            }
        }

        private void OnListViewDragOver(DragEventArgs e)
        {
            e.Handled = false;
            ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("Black"));
        }

        private void OnListViewDragLeave(DragEventArgs e)
        {
            ListViewBorderColor = (Brush)(new BrushConverter().ConvertFromString("LightGray"));
        }

        private void CheckBox_OnClick(object param)
        {
            bool isCheck;
            isCheck = (!SaveDirCheck) ? true : false;

            SaveDirCheck = isCheck;
            SaveDirControlEnabled = isCheck;
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
                    if (!CollectionFileList.Contains(file))
                        CollectionFileList.Add(file);
            }
        }

        private void DeleteFile_OnClick()
        {
            foreach (string item in SelectedListViewItems)
            {
                CollectionFileList.Remove(item);
            }
        }

        /// <summary>
        /// 코드 수정 후 변경!
        /// </summary>
        private async void ConvertProcess_OnClick()
        {
            await ((MetroWindow)Application.Current.MainWindow).ShowChildWindowAsync(new ProgressBarChildView());
            //if (CollectionFileList.Count != 0)
            //{
            //    //List<string> fileList = new List<string>(CollectionFileList);
            //    //foreach(string file in fileList)
            //    //{
            //    //    ConvertProcess(file);
            //    //    CollectionFileList.Remove(file);
            //    //    // 실시간 ui 업데이트
            //    //    // ((MetroWindow)Application.Current.MainWindow).Dispatcher.Invoke((ThreadStart)(()=>{ }), DispatcherPriority.ApplicationIdle);
            //    //}
            //    foreach (string filePath in CollectionFileList)
            //        ConvertProcess(filePath);

            //    CollectionFileList.Clear();
            //    MessageBox.Show("Convert complete.", "Complete");
            //} 
            //else
            //{
            //    MessageBox.Show("No files to convert.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }
        #endregion
        #endregion

        #region Progress Bar
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CurrentProgress = e.ProgressPercentage;
            //TextBlockText = (string)e.UserState;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0, String.Format("Processing Iteration 1."));
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                worker.ReportProgress((i + 1), String.Format("Processing Iteration {0}.", i + 2));
            }

            worker.ReportProgress(100, "Done Processing.");
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("All Done!");
            CurrentProgress = 0;
            //TextBlockText = "";
        }
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
