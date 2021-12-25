using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Server
{
    public class HelperServer
    {
        const string libsDataFileName = "LibsData.json";
        static List<UpdateLib> libsData = null;

        public static List<UpdateLib> Libs
        {
            get
            {
                if (libsData == null)
                {
                    var dir = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath));
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    var settingPath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, libsDataFileName)).FullName;
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new List<UpdateLib>() { };
                        File.WriteAllText(settingPath, temp.ToJson(true));
                    }
                    libsData = System.IO.File.ReadAllText(settingPath).JsonTo<List<UpdateLib>>();
                }
                return libsData;
            }
            set
            {
                libsData = value;
            }
        }

        public static void SaveLibsData()
        {
            var settingPath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, libsDataFileName)).FullName;
            File.WriteAllText(settingPath, Libs.ToJson(true));
        }


    }
}
