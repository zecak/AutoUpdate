using System;
using System.ComponentModel;

namespace Anye.Soft.Common
{
    public class FileVersion : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string FileName { get; set; } 
        public long FileSize { get; set; }
        public string MD5 { get; set; }

        public EnumUpdateAction Action { get; set; } = EnumUpdateAction.无;



    }
}
