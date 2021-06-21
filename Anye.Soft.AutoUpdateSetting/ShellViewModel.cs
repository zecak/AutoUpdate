using Anye.Soft.Common;
using Stylet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Anye.Soft.AutoUpdateSetting
{
    public class ShellViewModel : Conductor<IScreen>
    {
        public ObservableCollection<AppInfo> AppInfos { get; set; } = new ObservableCollection<AppInfo>();

        ICollectionView GetCollection(object obj)
        {
            return CollectionViewSource.GetDefaultView(obj);
        }

        public AppInfo CurAppInfo { get; set; }

        public List<string> EnumUpdateActionList { get; set; }
        public string CurEnumUpdateAction { get; set; }

        public void SelectionChanged(UpdateConfig updateConfig)
        {
            if (updateConfig == null) { return; }
            if (CurEnumUpdateAction == "必须")
            {
                GetCollection(updateConfig.FileVersions).Filter = x => x is FileVersion fileVersion && fileVersion.Action != EnumUpdateAction.无;
            }
            else if (CurEnumUpdateAction == "全部")
            {
                GetCollection(updateConfig.FileVersions).Filter = x => true;
            }
            else
            {
                GetCollection(updateConfig.FileVersions).Filter = x => x is FileVersion fileVersion && fileVersion.Action.ToString() == CurEnumUpdateAction;
            }

        }
        public ShellViewModel()
        {

            Load();

            var list = new List<string>();
            list.Add("必须");
            list.Add("全部");
            var strlist = Enum.GetValues(typeof(EnumUpdateAction));
            foreach (object item in Enum.GetValues(typeof(EnumUpdateAction)))
            {
                list.Add(item.ToString());
            }
            EnumUpdateActionList = list;
            CurEnumUpdateAction = "全部";
        }

        public void Load()
        {
            var dir_UpdatePath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath)); 
            if(!dir_UpdatePath.Exists)
            {
                dir_UpdatePath.Create();
            }

            var apps = Directory.GetDirectories(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath));
            if (apps.Length > 0)
            {
                AppInfos.Clear();

                foreach (var app in apps)
                {
                    var dir = new DirectoryInfo(app);
                    var appInfo = new AppInfo() { Name = dir.Name, Path = dir.FullName };

                    var vers = Directory.GetDirectories(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name));

                    if (vers.Length > 0)
                    {
                        vers = vers.Reverse().ToArray();
                        if (vers.Length == 1)
                        {
                            var ver = vers[0];
                            var dir_ver = new DirectoryInfo(ver);

                            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name, Helper.SettingAutoUpdateSetting.FileName);
                            if (!File.Exists(configPath))//配置文件不存在,则加载文件并生成配置文件
                            {
                                var files = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name), "*", SearchOption.AllDirectories);
                                if (files.Length > 0)
                                {
                                    var temp = new UpdateConfig() { ClientName = appInfo.Name, ClientVersion = dir_ver.Name.ToInt(0) };
                                    foreach (var file in files)
                                    {
                                        var fileInfo = new FileInfo(file);
                                        if (fileInfo.Name == Helper.SettingAutoUpdateSetting.FileName)
                                        {
                                            continue;
                                        }
                                        temp.FileVersions.Add(new FileVersion() { FileName = fileInfo.FullName.Replace(ver + @"\", ""), FileSize = fileInfo.Length, MD5 = file.MD5File(), Action = EnumUpdateAction.更新 });
                                    }
                                    File.WriteAllText(configPath, temp.ToJson());
                                    appInfo.Configs.Add(System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>());
                                }

                            }
                            else//否则直接加载配置文件
                            {
                                var config = System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>();
                                config.ClientName = appInfo.Name;
                                config.ClientVersion = dir_ver.Name.ToInt(config.ClientVersion);
                                appInfo.Configs.Add(config);
                            }
                        }
                        else
                        {
                            vers = vers.Take(2).ToArray();

                            UpdateConfig updateConfig_old = null;
                            {
                                //加载旧版信息
                                var ver = vers[1];
                                var dir_ver = new DirectoryInfo(ver);

                                var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name, Helper.SettingAutoUpdateSetting.FileName);
                                if (!File.Exists(configPath))//配置文件不存在,则加载文件并生成配置文件
                                {
                                    var files = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name), "*", SearchOption.AllDirectories);
                                    if (files.Length > 0)
                                    {
                                        var temp = new UpdateConfig() { ClientName = appInfo.Name, ClientVersion = dir_ver.Name.ToInt(0) };
                                        foreach (var file in files)
                                        {
                                            var fileInfo = new FileInfo(file);
                                            if (fileInfo.Name == Helper.SettingAutoUpdateSetting.FileName)
                                            {
                                                continue;
                                            }
                                            temp.FileVersions.Add(new FileVersion() { FileName = fileInfo.FullName.Replace(ver + @"\", ""), FileSize = fileInfo.Length, MD5 = file.MD5File(), Action = EnumUpdateAction.更新 });
                                        }
                                        File.WriteAllText(configPath, temp.ToJson());
                                        updateConfig_old = temp;

                                    }

                                }
                                else//否则直接加载配置文件
                                {
                                    updateConfig_old = System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>();
                                    updateConfig_old.ClientName = appInfo.Name;
                                    updateConfig_old.ClientVersion = dir_ver.Name.ToInt(updateConfig_old.ClientVersion);
                                }
                            }

                            {
                                //加载并生成新版信息
                                var ver = vers[0];
                                var dir_ver = new DirectoryInfo(ver);

                                var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name, Helper.SettingAutoUpdateSetting.FileName);
                                if (!File.Exists(configPath))//配置文件不存在,则加载文件并生成配置文件
                                {
                                    var files = Directory.GetFiles(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, appInfo.Name, dir_ver.Name), "*", SearchOption.AllDirectories);
                                    if (files.Length > 0)
                                    {
                                        var temp = new UpdateConfig() { ClientName = appInfo.Name, ClientVersion = dir_ver.Name.ToInt(0) };
                                        foreach (var file in files)
                                        {
                                            var fileInfo = new FileInfo(file);
                                            if (fileInfo.Name == Helper.SettingAutoUpdateSetting.FileName)
                                            {
                                                continue;
                                            }
                                            var fileVer = new FileVersion() { FileName = fileInfo.FullName.Replace(ver + @"\", ""), FileSize = fileInfo.Length, MD5 = file.MD5File() };
                                            temp.FileVersions.Add(fileVer);
                                        }

                                        if (updateConfig_old != null)
                                        {
                                            foreach (var fileVersion in temp.FileVersions)
                                            {
                                                var old_file = updateConfig_old.FileVersions.FirstOrDefault(m => m.FileName == fileVersion.FileName);
                                                if (old_file == null)
                                                {
                                                    fileVersion.Action = EnumUpdateAction.更新;
                                                }
                                                else
                                                {
                                                    if (old_file.MD5 == fileVersion.MD5)
                                                    {
                                                        fileVersion.Action = EnumUpdateAction.无;

                                                    }
                                                    else
                                                    {
                                                        fileVersion.Action = EnumUpdateAction.更新;
                                                    }
                                                }
                                            }

                                            foreach (var old_file in updateConfig_old.FileVersions)
                                            {
                                                var fileVersion = temp.FileVersions.FirstOrDefault(m => m.FileName == old_file.FileName);
                                                if (fileVersion == null)
                                                {
                                                    var fileVer = new FileVersion() { FileName = old_file.FileName, FileSize = 0, MD5 = "", Action = EnumUpdateAction.删除 };
                                                    temp.FileVersions.Add(fileVer);
                                                }
                                            }


                                        }

                                        File.WriteAllText(configPath, temp.ToJson());
                                        appInfo.Configs.Add(System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>());
                                    }

                                }
                                else//否则直接加载配置文件
                                {
                                    var config = System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>();
                                    config.ClientName = appInfo.Name;
                                    config.ClientVersion = dir_ver.Name.ToInt(config.ClientVersion);
                                    appInfo.Configs.Add(config);
                                }
                            }
                            if (updateConfig_old != null)
                            {
                                appInfo.Configs.Add(updateConfig_old);
                            }
                        }
                    }

                    AppInfos.Add(appInfo);
                }
                CurAppInfo = AppInfos.FirstOrDefault();
            }
        }

        public void SaveConfig(UpdateConfig updateConfig)
        {
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, updateConfig.ClientName, updateConfig.ClientVersion.ToString(), Helper.SettingAutoUpdateSetting.FileName);

            File.WriteAllText(configPath, updateConfig.ToJson());

            updateConfig = System.IO.File.ReadAllText(configPath).JsonTo<UpdateConfig>();

            System.Windows.MessageBox.Show("保存成功");
        }

        public void DelConfigAndLoad(UpdateConfig updateConfig)
        {
            var configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, updateConfig.ClientName, updateConfig.ClientVersion.ToString(), Helper.SettingAutoUpdateSetting.FileName);

            File.Delete(configPath);

            Load();
        }


        public void ShowFileVersionView(UpdateConfig updateConfig)
        {
            var file = new FileVersionViewModel(updateConfig);
            this.ActivateItem(file);
        }

    }
}
