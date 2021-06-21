using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate
{
    class Program
    {

        static Channel channel = null;
        static gRPC.gRPCClient client = null;
        static void Main(string[] args)
        {
            try
            {
                if (HelperClient.SettingAutoUpdate.IsUseAutoUpdateJson)
                {
                    var clientName = HelperClient.SettingAutoUpdate.ClientName;
                    var clientVersion = HelperClient.SettingAutoUpdate.ClientVersion;
                    var updatePath = HelperClient.SettingAutoUpdate.UpdatePath;
                    var fileName = HelperClient.SettingAutoUpdate.FileName;
                    var enumAutoUpdate = HelperClient.SettingAutoUpdate.EnumAutoUpdate;

                    if (clientName.IsNullOrEmptyOrSpace())
                    {
                        Helper.Log.Error("配置文件[ClientName]不能为空");
                        return;
                    }
                    if (fileName.IsNullOrEmptyOrSpace())
                    {
                        Helper.Log.Error("配置文件[FileName]不能为空");
                        return;
                    }

                    channel = new Channel(HelperClient.Setting.ServerIP + ":" + HelperClient.Setting.ServerPort, ChannelCredentials.Insecure);
                    client = new gRPC.gRPCClient(channel);

                    Task.Run(async () =>
                    {
                        await GetVersion(clientName, clientVersion, updatePath, fileName, enumAutoUpdate);
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    if (args.Length != 5)
                    {
                        Helper.Log.Info("[暗夜软件自动升级客户端]");
                        Helper.Log.Info("使用说明:");
                        Helper.Log.Info("第1个参数[客户端名称],如:Client");
                        Helper.Log.Info("第2个参数[客户端版本],如:1");
                        Helper.Log.Info("第3个参数[升级路径],如:\"..\\UpdatePath\"");
                        Helper.Log.Info("第4个参数[程序名称](用于关闭程序和启动程序,不含.exe后缀),如:\"myclient\"");
                        Helper.Log.Info("第5个参数[程序类型](普通程序:0,服务程序:1),如:0");
                        Helper.Log.Info("注:[升级路径]仅支持当前目录的相对目录");
                        Helper.Log.Info("   [程序名称]不要和本程序名称相同,否则本程序被关闭后,会导致升级失败");
                        return;
                    }
                    var clientName = args[0] ?? "";
                    var clientVersion = args[1].ToInt(0);
                    var updatePath = args[2] ?? "";
                    var fileName = args[3] ?? "";
                    EnumAutoUpdate enumAutoUpdate = args[4].IsNullOrEmptyOrSpace() ? EnumAutoUpdate.Client : (EnumAutoUpdate)args[4].ToInt();

                    if (clientName.IsNullOrEmptyOrSpace())
                    {
                        Helper.Log.Error("参数[客户端名称]不能为空");
                        return;
                    }
                    if (fileName.IsNullOrEmptyOrSpace())
                    {
                        Helper.Log.Error("参数[客户端进程名称]不能为空");
                        return;
                    }

                    channel = new Channel(HelperClient.Setting.ServerIP + ":" + HelperClient.Setting.ServerPort, ChannelCredentials.Insecure);
                    client = new gRPC.gRPCClient(channel);

                    Task.Run(async () =>
                    {
                        await GetVersion(clientName, clientVersion, updatePath, fileName, enumAutoUpdate);
                    }).GetAwaiter().GetResult();

                }
                
            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
            }

            if (HelperClient.SettingAutoUpdate.IsConsoleReadKey)
            {
                Helper.Log.Info("按任意键退出");
                Console.ReadKey();
            }
        }

        static async Task GetVersion(string clientName, int clientVersion, string updatePath, string fileName, EnumAutoUpdate enumAutoUpdate)
        {
            var chatCall = client.Chat();

            List<UpdateConfig> needUpdates = null;

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        needUpdates = note.Data.JsonTo<List<UpdateConfig>>();
                    }
                    Helper.Log.Debug("Msg:" + note.Msg);
                }

                if (needUpdates != null)
                {
                    Helper.Log.Info("GetVersion:" + needUpdates.ToJson());

                    await CloseApp(fileName, enumAutoUpdate);
                    await DownFiles(updatePath, needUpdates);
                    await StartApp(fileName, updatePath, enumAutoUpdate);
                    Helper.Log.Debug("程序更新完毕");
                }
            });

            try
            {
                var info = new AnyeSoft.Common.Service.APIRequest() { ApiPath = "/get/version", AppID = HelperClient.Setting.AppID, Data = new ApiVersionModel() { ClientName = clientName, ClientVersion = clientVersion }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + HelperClient.Setting.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                Helper.Log.Error(ex);
            }

            await responseReaderTask;
        }

        static async Task DownFiles(string updatePath, List<UpdateConfig> needUpdates)
        {
            foreach (var updateConfig in needUpdates)
            {
                var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, updatePath);
                Helper.Log.Debug("开始更新版本:" + updateConfig.ClientVersion);

                foreach (var fileVersion in updateConfig.FileVersions)
                {
                    if (fileVersion.Action == EnumUpdateAction.更新)
                    {
                        var chatCall = client.Chat();

                        List<AnyeSoft.Common.Service.APIReply> contentList = new List<AnyeSoft.Common.Service.APIReply>();

                        var responseReaderTask = Task.Run(async () =>
                        {
                            while (await chatCall.ResponseStream.MoveNext())
                            {
                                var note = chatCall.ResponseStream.Current;
                                contentList.Add(note);
                                Helper.Log.Debug("下载文件:" + fileVersion.FileName + "[" + note.FileBlock + "]");
                            }

                            var filepath = System.IO.Path.Combine(path, fileVersion.FileName);
                            var filepathInfo = new FileInfo(filepath);
                            if (!filepathInfo.Directory.Exists)
                            {
                                filepathInfo.Directory.Create();
                            }

                            FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite);
                            contentList.OrderBy(x => x.FileBlock).ToList().ForEach(x => x.FileContents.WriteTo(fs));
                            fs.Close();
                            Helper.Log.Info("更新文件:" + System.IO.Path.Combine(path, fileVersion.FileName));

                            var md5 = System.IO.Path.Combine(path, fileVersion.FileName).MD5File();
                            if (md5 == fileVersion.MD5)
                            {
                                Helper.Log.Debug("文件校验成功:" + System.IO.Path.Combine(path, fileVersion.FileName));
                            }
                            else
                            {
                                Helper.Log.Error("文件校验失败:" + System.IO.Path.Combine(path, fileVersion.FileName));
                            }


                        });

                        try
                        {
                            var info = new AnyeSoft.Common.Service.APIRequest() { ApiPath = "/file/down", AppID = HelperClient.Setting.AppID, Data = new ApiDownFileModel() { ClientName = updateConfig.ClientName, ClientVersion = updateConfig.ClientVersion, FileName = fileVersion.FileName }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                            info.Sign = (info.AppID + info.Data + info.Time + HelperClient.Setting.ServerKey).ToMd5();

                            await chatCall.RequestStream.WriteAsync(info);
                            await chatCall.RequestStream.CompleteAsync();
                        }
                        catch (RpcException ex)
                        {
                            Helper.Log.Error(ex);
                        }

                        await responseReaderTask;
                    }
                    else if (fileVersion.Action == EnumUpdateAction.删除)
                    {
                        var responseReaderTask = Task.Run(() =>
                        {
                            File.Delete(System.IO.Path.Combine(path, fileVersion.FileName));
                            Helper.Log.Info("删除文件:" + System.IO.Path.Combine(path, fileVersion.FileName));
                        });
                        await responseReaderTask;
                    }
                }
                HelperClient.SaveSettingAutoUpdate(updateConfig.ClientVersion);
                Helper.Log.Debug("更新本地版本配置文件");
            }
        }


        static async Task CloseApp(string name, EnumAutoUpdate enumAutoUpdate = EnumAutoUpdate.Client)
        {
            await Task.Run(() =>
            {
                switch (enumAutoUpdate)
                {
                    case EnumAutoUpdate.Client:
                        {
                            var processName = name;
                            Helper.Log.Debug("准备关闭程序:" + processName);
                            var plist = System.Diagnostics.Process.GetProcessesByName(processName);
                            foreach (System.Diagnostics.Process thisProc in plist)
                            {
                                try
                                {
                                    if (!thisProc.CloseMainWindow())
                                    {
                                        Helper.Log.Warn("CloseMainWindow失败:" + processName);
                                        thisProc.Kill();
                                    }
                                    thisProc.WaitForExit();
                                    Helper.Log.Info("已关闭程序:" + processName);
                                }
                                catch (Exception ex)
                                {
                                    Helper.Log.Error(ex);
                                }
                            }
                        }
                        break;
                    case EnumAutoUpdate.Service:
                        {
                            var hasService = HelperClient.IsWindowsServiceInstalled(name);
                            if (hasService)
                            {
                                HelperClient.StopService(name);
                            }
                        }
                        break;
                    case EnumAutoUpdate.Web:
                        break;
                    default:
                        break;
                }

            });

        }

        static async Task StartApp(string name, string path, EnumAutoUpdate enumAutoUpdate = EnumAutoUpdate.Client)
        {
            await Task.Run(() =>
            {
                switch (enumAutoUpdate)
                {
                    case EnumAutoUpdate.Client:
                        {
                            Helper.Log.Debug("准备运行程序:" + name);
                            var fullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path, name + ".exe");
                            if (File.Exists(fullName))
                            {
                                Helper.Log.Debug("运行程序:" + fullName);
                                System.Diagnostics.Process p = new System.Diagnostics.Process();
                                p.StartInfo.FileName = fullName;
                                p.Start();
                                Helper.Log.Debug("已运行程序:" + name);
                            }
                            else
                            {
                                Helper.Log.Debug("运行程序不存在:" + fullName);
                            }
                        }
                        break;
                    case EnumAutoUpdate.Service:
                        {
                            var hasService = HelperClient.IsWindowsServiceInstalled(name);
                            if (hasService)
                            {
                                HelperClient.ReStartService(name);
                            }
                        }
                        break;
                    case EnumAutoUpdate.Web:
                        break;
                    default:
                        break;
                }
            });

        }

    }
}
