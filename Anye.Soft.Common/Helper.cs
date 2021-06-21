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
                        var temp = new ServerSetting() { DisplayName = "暗夜软件自动更新服务", ServiceName = "Anye.Soft.AutoUpdateServer", Description = "暗夜软件自动更新服务", ServerIP = "127.0.0.1", ServerPort = "9999", ServerKey = "20210615" };
                        File.WriteAllText(settingPath, temp.ToJson());
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
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingAutoUpdateSettingFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new AutoUpdateSettingModel() { UpdatePath="Clients", FileName= "AutoUpdateConfig.json" };
                        File.WriteAllText(settingPath, temp.ToJson());
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
