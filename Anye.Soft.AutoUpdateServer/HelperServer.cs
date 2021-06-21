using Anye.Soft.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdateServer
{
    public class HelperServer
    {
        public static List<UpdateConfig> UpdateConfigs { get; set; } = new List<UpdateConfig>();

        static DirectoryMonitor directoryMonitor;
        public static void MonitorDirectory()
        {
            var files = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath), Helper.SettingAutoUpdateSetting.FileName, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var config = System.IO.File.ReadAllText(file).JsonTo<UpdateConfig>();
                if (config != null)
                {
                    UpdateConfigs.Add(config);
                }
            }

            Helper.Log.Debug("UpdateConfigs:" + UpdateConfigs.ToJson());

            directoryMonitor = new DirectoryMonitor(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath), Helper.SettingAutoUpdateSetting.FileName, true);
            directoryMonitor.Change += DirectoryMonitor_Change;
            directoryMonitor.Start();
        }

        private static void DirectoryMonitor_Change(string _path)
        {
            Helper.Log.Debug("文件变动:" + _path + "");
            var fileInfo = new FileInfo(_path);
            var rootUpdateSoftPath = fileInfo.Directory?.Parent?.Parent?.FullName;
            if (System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath) == rootUpdateSoftPath)
            {
                var versionName = fileInfo.Directory.Name;
                var clientName = fileInfo.Directory.Parent.Name;

                var config = System.IO.File.ReadAllText(fileInfo.FullName).JsonTo<UpdateConfig>();
                if (config != null)
                {
                    var updateConfig = UpdateConfigs.FirstOrDefault(m => m.ClientName == config.ClientName && m.ClientVersion == config.ClientVersion);
                    if (updateConfig != null)
                    {
                        UpdateConfigs.Remove(updateConfig);
                    }
                    UpdateConfigs.Add(config);

                    Helper.Log.Debug("UpdateConfigs:" + UpdateConfigs.ToJson());
                }
            }

        }
    }
}
