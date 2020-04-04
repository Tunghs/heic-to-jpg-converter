using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SHControls.Controls
{
    [TemplatePart(Name = PART_ListView, Type = typeof(ListView))]
    public class FileDragDropListView : Control
    {
        const string PART_ListView = "PART_ListView";
    }
}
