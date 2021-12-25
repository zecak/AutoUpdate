using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage
{
    public class MHelper
    {
        public const string manageDataFileName = "manageData.json";
        static ObservableCollection<ManageDataModel> manageDataConfig = null;

        public static ObservableCollection<ManageDataModel> ManageDataConfig
        {
            get
            {

                if (manageDataConfig == null)
                {
                    var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
                    if (!System.IO.Directory.Exists(dataDir))
                    {
                        System.IO.Directory.CreateDirectory(dataDir);
                    }
                    var settingPath = System.IO.Path.Combine(dataDir, manageDataFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        manageDataConfig = new ObservableCollection<ManageDataModel>() { };
                        File.WriteAllText(settingPath, manageDataConfig.ToJson(true));
                    }
                    manageDataConfig = System.IO.File.ReadAllText(settingPath).JsonTo<ObservableCollection<ManageDataModel>>();
                }
                return manageDataConfig;
            }
            set
            {
                manageDataConfig = value;
            }
        }

        public static void SaveManageDataConfig()
        {
            if (ManageDataConfig == null) { return; }
            var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            if (!System.IO.Directory.Exists(dataDir))
            {
                return;
            }
            var settingPath = System.IO.Path.Combine(dataDir, manageDataFileName);
            File.WriteAllText(settingPath, ManageDataConfig.ToJson(true));
        }
        public static void LoadManageDataConfig()
        {
            var dataDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            if (!System.IO.Directory.Exists(dataDir))
            {
                System.IO.Directory.CreateDirectory(dataDir);
            }
            var settingPath = System.IO.Path.Combine(dataDir, manageDataFileName);
            var list = System.IO.File.ReadAllText(settingPath).JsonTo<ObservableCollection<ManageDataModel>>();
            ManageDataConfig.Clear();
            foreach (var item in list)
            {
                ManageDataConfig.Add(item);
            }
        }

        public static string AddManageData(ManageDataModel manageData)
        {
            if (manageData == null) { return "新增数据不能为空"; }
            manageData.GID = Guid.NewGuid();
            var model = ManageDataConfig.FirstOrDefault(m => m.Name == manageData.Name);
            if (model == null)
            {
                ManageDataConfig.Add(manageData);
                SaveManageDataConfig();
                return "";
            }
            else
            {
                return "该数据已存在:"+ manageData.Name;
            }
        }

        public static string EditManageData(ManageDataModel manageData)
        {
            if (manageData == null) { return "编辑数据不能为空"; }
            var model = ManageDataConfig.FirstOrDefault(m => m.Name == manageData.Name);
            if (model != null)
            {
                model.Name = manageData.Name;
                model.IPort = manageData.IPort;
                model.Remarks = manageData.Remarks;
                model.AdminName = manageData.AdminName;
                model.ServerKey = manageData.ServerKey;
                SaveManageDataConfig();
                return "";
            }
            else
            {
                return "该数据不存在";
            }
        }

        public static string DelManageData(ManageDataModel manageData)
        {
            if (manageData == null) { return "删除数据不能为空"; }
            var model = ManageDataConfig.FirstOrDefault(m => m.GID == manageData.GID);
            if (model != null)
            {
                ManageDataConfig.Remove(model);
            }

            SaveManageDataConfig();
            return "";
        }

    }
}
