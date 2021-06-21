using Anye.Soft.Common;
using AnyeSoft.Common.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate
{
    public class HelperClient
    {
        const string settingFileName = "client.json";
        static ClientSetting settingjson = null;

        public static ClientSetting Setting
        {
            get
            {
                if (settingjson == null)
                {
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new ClientSetting() { ServerIP = "127.0.0.1", ServerPort = "9999", AppID = "Anye", ServerKey = "20210615" };
                        File.WriteAllText(settingPath, temp.ToJson());
                    }
                    settingjson = System.IO.File.ReadAllText(settingPath).JsonTo<ClientSetting>();
                }
                return settingjson;
            }
            set
            {
                settingjson = value;
            }
        }

        const string settingAutoUpdateFileName = "AutoUpdate.json";
        static AutoUpdateModel settingAutoUpdate = null;

        public static AutoUpdateModel SettingAutoUpdate
        {
            get
            {
                if (settingAutoUpdate == null)
                {
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingAutoUpdateFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new AutoUpdateModel() { ClientName = "App1", ClientVersion = 0, UpdatePath = "Client", FileName = "AnyeSoft.Client", IsUseAutoUpdateJson = false, EnumAutoUpdate = EnumAutoUpdate.Client, IsConsoleReadKey = true };
                        File.WriteAllText(settingPath, temp.ToJson());
                    }
                    settingAutoUpdate = System.IO.File.ReadAllText(settingPath).JsonTo<AutoUpdateModel>();
                }
                return settingAutoUpdate;
            }
            set
            {
                settingAutoUpdate = value;
            }
        }

        public static void SaveSettingAutoUpdate(int clientVersion)
        {
            SettingAutoUpdate.ClientVersion = clientVersion;
            var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingAutoUpdateFileName);
            File.WriteAllText(settingPath, SettingAutoUpdate.ToJson());
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        /// <summary>  
        /// 判断是否安装了某个服务  
        /// </summary>  
        /// <param name="serviceName"></param>  
        /// <returns></returns>  
        public static bool IsWindowsServiceInstalled(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName.ToLower() == serviceName.ToLower())
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            { return false; }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        /// <summary>  
        /// 启动某个服务  
        /// </summary>  
        /// <param name="serviceName"></param>  
        public static void ReStartService(string serviceName)
        {
            try
            {
                StopService(serviceName);
                ServiceController serviceController = new ServiceController(serviceName);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch { }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        /// <summary>  
        /// 停止某个服务  
        /// </summary>  
        /// <param name="serviceName"></param>  
        public static void StopService(string serviceName)
        {
            try
            {
                ServiceController serviceController = new ServiceController(serviceName);
                if (serviceController.CanStop)
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch { }
        }

    }
}
