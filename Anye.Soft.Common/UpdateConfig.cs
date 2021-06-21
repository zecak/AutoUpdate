using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common
{
    public class UpdateConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ClientName { get; set; }
        public int ClientVersion { get; set; }
        public ObservableCollection<FileVersion> FileVersions { get; set; } = new ObservableCollection<FileVersion>();
        public FileVersion FileVersionModel { get; set; }
        public void AddFileVersion(FileVersion fileVersion)
        {
            if (fileVersion == null) { return; }
            FileVersions?.Add(fileVersion);
        }

        public void DelFileVersion()
        {
            //FileVersion fileVersion = obj as FileVersion;
            if (FileVersionModel == null) { return; }
            var f = FileVersions?.FirstOrDefault(m => m.FileName == FileVersionModel.FileName);
            if (f != null)
            {
                FileVersions?.Remove(f);
                FileVersionModel = null;
            }
        }
    }
}
