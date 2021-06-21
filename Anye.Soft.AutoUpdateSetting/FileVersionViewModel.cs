using Anye.Soft.Common;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdateSetting
{
    public class FileVersionViewModel : Screen
    {
        public UpdateConfig UpdateConfig { get; set; }
        public FileVersion FileVersion { get; set; } = new FileVersion();

        public FileVersionViewModel()
        {

        }

        public FileVersionViewModel(UpdateConfig updateConfig)
        {
            UpdateConfig = updateConfig;
        }
        public void Close()
        {
            this.RequestClose();
        }

        public void Add()
        {
            if (FileVersion.FileName.IsNullOrEmptyOrSpace())
            {
                System.Windows.MessageBox.Show("文件名必填");
                return;
            }
            var file = UpdateConfig?.FileVersions.FirstOrDefault(m => m.FileName == FileVersion.FileName);
            if (file != null)
            {
                System.Windows.MessageBox.Show("文件已存在");
                return;
            }
            UpdateConfig?.FileVersions.Add(FileVersion);
            Close();
        }

        public void BrowseFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UpdateSoft", UpdateConfig.ClientName, UpdateConfig.ClientVersion.ToString());
            var rez = openFileDialog.ShowDialog();
            if (rez == true)
            {
                var ver = new FileVersion();
                var finfo = new FileInfo(openFileDialog.FileName);
                ver.Action = EnumUpdateAction.更新;
                ver.FileName = finfo.Name;
                ver.FileSize = finfo.Length;
                ver.MD5 = finfo.FullName.MD5File();
                FileVersion = ver;
            }
        }
    }
}
