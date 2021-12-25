using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using MahApps.Metro.Controls;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    /// <summary>
    /// AddVersionLibView.xaml 的交互逻辑
    /// </summary>
    public partial class AddVersionLibView : MetroWindow
    {
        public AddVersionLibView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {

                var dirInfo = new DirectoryInfo(files[0]);
                if (dirInfo.Attributes == FileAttributes.Directory)
                {
                    var txt = sender as TextBox;
                    txt.Text = files[0];
                }
                
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }
}
