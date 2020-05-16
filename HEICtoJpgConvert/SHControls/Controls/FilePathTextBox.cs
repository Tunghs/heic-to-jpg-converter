using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SHControls.Controls
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    public class FilePathTextBox : Control
    {
        const string PART_TextBox = "PART_TextBox";
        const string PART_Button = "PART_Button";

        #region Dependency Property
        public static readonly DependencyProperty ControlNameProperty = DependencyProperty.Register("ControlName", typeof(string), typeof(FilePathTextBox),
            new PropertyMetadata(null, null));

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FilePathTextBox),
            new PropertyMetadata(null, null));

        public static readonly DependencyProperty AvailableExtProperty = DependencyProperty.Register("AvailableExt", typeof(ObservableCollection<string>), typeof(FilePathTextBox),
            new PropertyMetadata(null, null));

        public static readonly DependencyProperty ISFolderProperty = DependencyProperty.Register("ISFolder", typeof(bool), typeof(FilePathTextBox),
            new PropertyMetadata(false, null));
        public static readonly DependencyProperty ControlNameWidthProperty = DependencyProperty.Register("ControlNameWidth", typeof(int), typeof(FilePathTextBox),
            new PropertyMetadata(100, null));

        #endregion

        #region  Fields
        protected TextBox textBox = null;
        protected Button button = null;

        #endregion

        #region  Properties 
        public string ControlName
        {
            get { return (string)GetValue(ControlNameProperty); }
            set { SetValue(ControlNameProperty, value); }
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public ObservableCollection<string> AvailableExt
        {
            get { return (ObservableCollection<string>)GetValue(AvailableExtProperty); }
            set { SetValue(AvailableExtProperty, value); }
        }

        public bool ISFolder
        {
            get { return (bool)GetValue(ISFolderProperty); }
            set { SetValue(ISFolderProperty, value); }
        }
        public int ControlNameWidth
        {
            get { return (int)GetValue(ControlNameWidthProperty); }
            set { SetValue(ControlNameWidthProperty, value); }
        }
        #endregion

        #region Public Mathod
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            textBox = Template.FindName(PART_TextBox, this) as TextBox;
            if (textBox != null)
            {
                // binding mouse event
                textBox.PreviewDragOver += TextBox_PreviewDragOver;
                textBox.Drop += TextBox_Drop;
            }
            button = Template.FindName(PART_Button, this) as Button;
            if (button != null)
            {
                // binding mouse event
                button.Click += Button_Click;
            }
        }


        #endregion

        #region Private Mathod
        private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //int a = 0;
            //FilePathTextBox textBox = d as FilePathTextBox;
            //string test = textBox.FilePath;

        }


        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = true;
            }
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (ISFolder == true)
                {
                    if (IsFolder(files[0]) == true)
                        FilePath = files[0];
                }
                else
                {
                    if (AvailableExt == null)
                    {
                        FilePath = files[0];
                    }
                    else
                    {
                        foreach (string str in AvailableExt)
                        {
                            if (GetExtension(files[0]).ToLower() == str.ToLower())
                                FilePath = files[0];
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            if (ISFolder == true)
            {
                dialog.IsFolderPicker = true;
            }
            else
            {
                dialog.IsFolderPicker = false;
                if (AvailableExt != null)
                {
                    foreach (string str in AvailableExt)
                    {
                        dialog.Filters.Add(new CommonFileDialogFilter("Available Extention", $"*.{str}"));
                    }
                }
            }

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FilePath = dialog.FileName;
            }
        }

        private string GetExtension(string FullName)
        {
            if (FullName == null)
                return null;

            string extension = "";

            extension = FullName.Substring
                (
                FullName.LastIndexOf(".") + 1,
                FullName.Length - FullName.LastIndexOf(".") - 1
                );

            return extension;
        }

        private bool IsFolder(string FullPath)
        {
            if (FullPath == null)
                return false;

            FileAttributes attr = File.GetAttributes(FullPath);

            //Folder일 경우
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            else
                return false;
        }
        #endregion
    }
}
