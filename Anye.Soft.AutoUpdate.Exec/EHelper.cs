using Anye.Soft.AutoUpdate.Exec.Command;
using Anye.Soft.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec
{
    public class EHelper
    {
        public const string VersionFileName = "version.json";

        static object dataLock = new object();
        public const string DataFileName = "Data.json";
        static List<AddOptions> data = null;

        public static List<AddOptions> Data
        {
            get
            {
                if (data == null)
                {
                    var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName);
                    if (!System.IO.File.Exists(settingPath))
                    {
                        var temp = new List<AddOptions>() { };
                        File.WriteAllText(settingPath, temp.ToJson(true));
                    }
                    data = System.IO.File.ReadAllText(settingPath).JsonTo<List<AddOptions>>();
                }
                return data;
            }
            set
            {
                data = value;
            }
        }

        public static void SaveData()
        {
            lock (dataLock)
            {
                var settingPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DataFileName);
                File.WriteAllText(settingPath, Data.ToJson(true));
            }

        }

        /// <summary>
        /// 应用备份目录
        /// </summary>
        public const string AppsBak = "../AppsBak/";

        /// <summary>
        /// 下载临时目录
        /// </summary>
        public const string DownTemp = "../DownTemp/";


        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs=true)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "源目录不存在或找不到: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();   
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // 如果复制子目录,则重新指定新的源目录和新的目标目录
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
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

        [System.Runtime.Versioning.SupportedOSPlatform("linux")]
        public static List<PInfo> GetPInfos(string execname, string firstarg = "")
        {
            ProcessStartInfo startInfo;
            execname = " -a " + execname;
            startInfo = new ProcessStartInfo("pgrep", execname)
            {
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                ErrorDialog = false,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);
            List<PInfo> pInfos = new();
            using (StreamReader standardOutput = process.StandardOutput)
            {
                while (!standardOutput.EndOfStream)
                {
                    string text = standardOutput.ReadLine();
                    if (text.Contains("forbidden"))
                    {
                        process.Kill();
                        process.WaitForExit();
                        throw new Exception("权限不足");
                    }
                    string[] ts = text.Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (ts.Length == 3)
                    {
                        PInfo pInfo = new PInfo() { PID = int.Parse(ts[0]), PName = ts[1], PCmd = ts[2] };
                        pInfos.Add(pInfo);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(firstarg))
            {
                pInfos = pInfos.FindAll(m => m.PCmd.Contains(firstarg));
            }
            return pInfos;
        }

        [System.Runtime.Versioning.SupportedOSPlatform("linux")]
        public static string StopServiceLinux(string name)
        {
            ProcessStartInfo startInfo;
            startInfo = new ProcessStartInfo("systemctl", " stop " + name)
            {
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                ErrorDialog = false,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);
            string str = "";
            using (StreamReader standardOutput = process.StandardOutput)
            {
                while (!standardOutput.EndOfStream)
                {
                    string text = standardOutput.ReadLine();
                    str += text;
                }
            }
            return str;
        }

        [System.Runtime.Versioning.SupportedOSPlatform("linux")]
        public static string StartServiceLinux(string name)
        {
            ProcessStartInfo startInfo;
            startInfo = new ProcessStartInfo("systemctl", " start " + name)
            {
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                ErrorDialog = false,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);
            string str = "";
            using (StreamReader standardOutput = process.StandardOutput)
            {
                while (!standardOutput.EndOfStream)
                {
                    string text = standardOutput.ReadLine();
                    str += text;
                }
            }
            return str;
        }

        /// <summary>
        /// 启动程序,如:"bash run.sh"或"dotnet test.dll"
        /// </summary>
        /// <param name="execname">主程序(可空,空则直接执行要启动的程序)</param>
        /// <param name="firstname">要启动的程序</param>
        /// <param name="args">启动参数</param>
        [System.Runtime.Versioning.SupportedOSPlatform("linux")]
        public static void StartClient(string execname, string firstname, string args = "", string backStr = " & ")
        {
            ProcessStartInfo startInfo;
            if (string.IsNullOrWhiteSpace(execname))
            {
                startInfo = new ProcessStartInfo(firstname, " " + args + " " + backStr)
                {
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    ErrorDialog = false,
                    RedirectStandardError = true
                };
            }
            else
            {
                startInfo = new ProcessStartInfo(execname, " " + firstname + " " + args + " " + backStr)
                {
                    RedirectStandardOutput = true,
                    CreateNoWindow = false,
                    ErrorDialog = false,
                    RedirectStandardError = true
                };
            }

            Process.Start(startInfo);
        }
    }
}
