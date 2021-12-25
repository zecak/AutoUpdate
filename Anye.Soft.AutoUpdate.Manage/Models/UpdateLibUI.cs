using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Models
{
    public class UpdateLibUI: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 更新库名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 最后版本,用于创建版本库自动累加
        /// </summary>
        public int LastVersion { get; set; }
        public ObservableCollection<LibVersionInfoUI> Configs { get; set; } = new ObservableCollection<LibVersionInfoUI>();
        /// <summary>
        /// 界面选中的版本
        /// </summary>
        public LibVersionInfoUI LibVersionInfoUI { get; set; }

        public LibVersionInfoUI GetLastReleaseLibVersionInfoUI()
        {
            return Configs.FirstOrDefault(m => m.IsRelease || (m.IsEnablePreReleaseTime && DateTime.Now >= m.PreReleaseTime));
        }
    }
}
