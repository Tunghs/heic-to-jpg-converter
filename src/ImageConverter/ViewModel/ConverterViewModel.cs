using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ImageConverter.Bases;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ImageConverter.ViewModel
{
    public partial class ConverterViewModel : ViewModelBase
    {
        // private IFileControlService _controller;


    

        [ObservableProperty]
        private ObservableCollection<string> _images = new ObservableCollection<string>();

        [ObservableProperty]
        private string _imagePath;

        [ObservableProperty]
        private bool _isSaveDirectory;

        [ObservableProperty]
        private string _saveDirectoryPath;

        public ConverterViewModel()
        {

        }

        [RelayCommand]
        private void OnLoadImageBtnClick()
        {
            var fileDialog = new OpenFileDialog
            {
                Title = "Select Image",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Filter = "HEIF files (*.heif) | *.heif"
            };

            if (fileDialog.ShowDialog() == true)
            {
                var files = fileDialog.FileNames;
                for (int imgIdx = 0; imgIdx < files.Length; imgIdx++)
                {
                    Images.Add(files[imgIdx]);
                }
            }
        }

        [RelayCommand]
        private void OnClearBtnClick()
        {
            Images.Clear();
        }

        [RelayCommand]
        public void OnFileDrop(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            var dropItems = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int index = 0; index < dropItems.Length; index++)
            {
                if (IsDirectory(dropItems[index]))
                {
                    continue;
                }

                if (dropItems[index].ToLower().EndsWith("heif"))
                    Images.Add(dropItems[index]);
            }
        }

        [RelayCommand]
        public void OnOpenDirectoryExplorerBtnClick()
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Folder",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Multiselect = false
            };

            if (folderDialog.ShowDialog() == true)
            {
                SaveDirectoryPath = folderDialog.FolderName;
            }
        }

        [RelayCommand]
        public void OnConvertBtnClick()
        {
            Convert();
        }

        private bool IsDirectory(string path)
        {
            if (path == null)
            {
                return false;
            }

            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void Convert()
        {

        }
    }
}
