using Anye.Soft.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common
{
    public class Helper
    {

        static log4net.ILog log = null;
        public static log4net.ILog Log
        {
            get
            {

                if (log == null)
                {
                    var logCfg = new System.IO.FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config"));
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(logCfg);
                    log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                }

                return log;
            }
        }

        const string settingFileName = "server.json";
        static ServerSetting settingjson = null;

        public static ServerSetting ServerSetting
        {
            get
            {
                if (settingjson == null)
                {
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new ServerSetting() { ServerIP = "0.0.0.0", ServerPort = "9999", Admin = new UserModel() { Name = "anye", Pass = "20210615" }, Updater = new UserModel() { Name = "updater", Pass = "123456" } };
                        File.WriteAllText(settingPath, temp.ToJson(true));
                    }
                    settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<ServerSetting>();
                }
                return settingjson;
            }
            set
            {
                settingjson = value;
            }
        }


        const string settingAutoUpdateSettingFileName = "AutoUpdateSetting.json";
        static AutoUpdateSettingModel settingAutoUpdateSettingjson = null;

        public static AutoUpdateSettingModel SettingAutoUpdateSetting
        {
            get
            {
                if (settingAutoUpdateSettingjson == null)
                {
                    var settingPath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingAutoUpdateSettingFileName)).FullName;
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new AutoUpdateSettingModel() { UpdatePath = "../UpdateLibs/", FileName = "AutoUpdateConfig.json" };
                        File.WriteAllText(settingPath, temp.ToJson(true));
                    }
                    settingAutoUpdateSettingjson = System.IO.File.ReadAllText(settingPath).JsonTo<AutoUpdateSettingModel>();
                }
                return settingAutoUpdateSettingjson;
            }
            set
            {
                settingAutoUpdateSettingjson = value;
            }
        }

    }
}
