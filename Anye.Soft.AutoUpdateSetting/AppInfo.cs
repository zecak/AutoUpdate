using Anye.Soft.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdateSetting
{
    public class AppInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; }
        public string Path { get; set; }

        public ObservableCollection<UpdateConfig> Configs { get; set; } = new ObservableCollection<UpdateConfig>();
    }
}
