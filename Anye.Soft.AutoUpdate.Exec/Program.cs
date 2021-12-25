using Anye.Soft.AutoUpdate.Exec.Command;
using Anye.Soft.AutoUpdate.Exec.Common;
using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using CommandLine;
using CommandLine.Text;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Helper.Log.Debug("args:"+ args.ToJson());

                SentenceBuilder.Factory = () => new SentenceBuilderCN();

                var parser = new Parser(with => { });
                var parserResult = parser.ParseArguments<DefaultOptions, AddOptions, EditOptions, DelOptions, GetOptions, UpdateOptions, RunOptions, UpdSelfOptions>(args);
                parserResult
                    .WithParsed<DefaultOptions>(options => Default(options))
                    .WithParsed<AddOptions>(options => Add(options))
                    .WithParsed<EditOptions>(options => Edit(options))
                    .WithParsed<DelOptions>(options => Del(options))
                    .WithParsed<GetOptions>(options => Get(options))
                    .WithParsed<UpdateOptions>(options => Update(options))
                    .WithParsed<RunOptions>(options => Run(options))
                    .WithParsed<UpdSelfOptions>(options => UpdSelf(options))
                    .WithNotParsed(errs => DisplayHelp(parserResult, errs))
                  ;

            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
            }

        }

        private static void UpdSelf(UpdSelfOptions options)
        {
            Helper.Log.Debug("开始自我更新(由备份应用执行)");
            var channel = new Channel(options.Target, ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);
            Task.Run(async () =>
            {
                await GetAllVersionUpdSelf(client, options);
            }).GetAwaiter().GetResult();
        }

        private static async Task GetAllVersionUpdSelf(gRPC.gRPCClient client, UpdSelfOptions options)
        {
            var spath = new DirectoryInfo(System.IO.Path.Combine(options.BasePath, options.Path)).FullName;

            var chatCall = client.Chat();

            List<LibVersionInfo> needUpdates = null;

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        needUpdates = note.Data.JsonTo<List<LibVersionInfo>>();
                    }
                }

                if (needUpdates != null && needUpdates.Count > 0)
                {
                    //准备升级
                    Helper.Log.Debug("开始升级更新");
                    await UpdateAppUpdSelf(client, options, needUpdates, options.BasePath, options.Path, options.DownUpateLibPath, options.DestPath);
                    Helper.Log.Debug("程序更新完毕");
                }
                else
                {
                    Helper.Log.Debug("无需更新");

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (!options.IsService)
                        {
                            if (!string.IsNullOrWhiteSpace(options.FileName))
                            {
                                var processName = options.FileName.Substring(0, options.FileName.LastIndexOf("."));
                                var plist = System.Diagnostics.Process.GetProcessesByName(processName);
                                if (plist.Length == 0)
                                {
                                    var bashPath = AppDomain.CurrentDomain.BaseDirectory;

                                    var updatePath = options.Path;
                                    if (string.IsNullOrWhiteSpace(updatePath))
                                    {
                                        updatePath = options.UpdateLibName;
                                    }

                                    if (Path.IsPathRooted(updatePath))//是绝对路径,则bashPath设为空串
                                    {
                                        bashPath = "";
                                    }

                                    var fullName = System.IO.Path.Combine(bashPath, updatePath, options.FileName);
                                    if (File.Exists(fullName))
                                    {
                                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                                        p.StartInfo.FileName = fullName;
                                        p.StartInfo.Arguments = options.Args ?? "";
                                        p.StartInfo.WorkingDirectory = System.IO.Path.Combine(bashPath, updatePath);
                                        p.Start();
                                    }
                                }

                            }
                        }
                    }
                }
            });

            try
            {
                //获取本地版本号
                var settingPath = System.IO.Path.Combine(spath, EHelper.VersionFileName);
                VersionModel versionModel = null;
                if (File.Exists(settingPath))
                {
                    versionModel = File.ReadAllText(settingPath).JsonTo<VersionModel>();
                }
                var cver = options.CurrentVersion;
                if (versionModel != null)
                {
                    cver = versionModel.CurrentVersion;
                }

                var info = new AnyeSoft.Common.Service.APIRequest() { ApiPath = ApiPaths.GetAllVersion, AppID = options.User, Data = new ApiVersionModel() { UpdateLibName = options.UpdateLibName, Version = cver }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + options.Key).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                Helper.Log.Error(ex);
            }

            await responseReaderTask;
        }

        private static async Task UpdateAppUpdSelf(gRPC.gRPCClient client, UpdSelfOptions options, List<LibVersionInfo> needUpdates, string bashPath, string updatePath, string downUpateLibPath, string dpath)
        {
            var aopt = options.ToJson().JsonTo<AddOptions>();
            await UpdateApp(client, aopt, needUpdates, options.BasePath, options.Path, options.DownUpateLibPath, options.DestPath, false, false);
        }

        private static void Run(RunOptions options)
        {
            var json = options.ToJson();
            var t = json.JsonTo<AddOptions>();
            var channel = new Channel(options.Target, ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);
            Task.Run(async () =>
            {
                await GetAllVersion(client, t, true);
            }).GetAwaiter().GetResult();
        }

        private static void Update(UpdateOptions options)
        {
            var t = EHelper.Data.FirstOrDefault(m => m.Name == options.Name);
            if (t == null)
            {
                Console.WriteLine("更新库配置:" + options.Name + ",不存在");
                return;
            }
            var channel = new Channel(t.Target, ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);
            Task.Run(async () =>
            {
                await GetAllVersion(client, t);
            }).GetAwaiter().GetResult();
        }

        static async Task GetAllVersion(gRPC.gRPCClient client, AddOptions options, bool isexec = false)
        {

            var bashPath = AppDomain.CurrentDomain.BaseDirectory;
            var updatePath = options.Path;
            if (string.IsNullOrWhiteSpace(updatePath))
            {
                updatePath = options.UpdateLibName;
            }
            if (Path.IsPathRooted(updatePath))//是绝对路径,则bashPath设为空串
            {
                bashPath = "";
            }

            var spath = new DirectoryInfo(System.IO.Path.Combine(bashPath, updatePath)).FullName;
            var dpath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EHelper.AppsBak, options.UpdateLibName)).FullName;//目标
            var downPath = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EHelper.DownTemp)).FullName;
            var downUpateLibPath = new DirectoryInfo(System.IO.Path.Combine(downPath, options.UpdateLibName)).FullName;

            var myfullname = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var dfullname = Path.GetFileNameWithoutExtension(System.IO.Path.Combine(bashPath, updatePath, options.FileName));
            var isUpdateMe = myfullname == dfullname;
            
            var chatCall = client.Chat();

            List<LibVersionInfo> needUpdates = null;

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        needUpdates = note.Data.JsonTo<List<LibVersionInfo>>();
                    }
                }

                if (needUpdates != null && needUpdates.Count > 0)
                {
                    if (isUpdateMe)
                    {
                        //升级自己
                        //将应用备份到备份目录
                        if (System.IO.Directory.Exists(spath))
                        {
                            if (Directory.Exists(dpath))
                            {
                                Helper.Log.Debug("清理之前备份:" + dpath);
                                Directory.Delete(dpath, true);
                                Helper.Log.Debug("清理备份成功:" + dpath);
                            }

                            Helper.Log.Debug("开始备份:" + spath);
                            EHelper.DirectoryCopy(spath, dpath);
                            Helper.Log.Debug("备份成功:" + dpath);
                        }

                        //将更新操作交接给备份应用执行
                        Helper.Log.Debug("将更新操作交接给备份应用执行!");
                        //启动备份程序
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            var fullname = Path.Combine(dpath, options.FileName);
                            if (File.Exists(fullname))
                            {
                                Helper.Log.Debug("准备启动备份程序:" + fullname);
                                var args = " updself -t " + options.Target + " -u " + options.User + " -k " + options.Key + " -l " + options.UpdateLibName + " -p " + updatePath + " -e " + options.ExecName + " -f " + options.FileName + " -b " + bashPath + " -d " + downUpateLibPath + " -m " + dpath + " ";
                                Helper.Log.Debug("启动参数:" + args);
                                EHelper.StartClient("dotnet", fullname, args);
                                Helper.Log.Debug("备份程序已启动:" + options.FileName);
                            }
                        }
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            var fullname = Path.Combine(dpath, options.FileName);
                            if (File.Exists(fullname))
                            {
                                Helper.Log.Debug("准备运行备份程序:" + fullname);
                                var args = " updself -t " + options.Target + " -u " + options.User + " -k " + options.Key + " -l " + options.UpdateLibName + " -p " + updatePath + " -f " + options.FileName + " -b " + bashPath + " -d " + downUpateLibPath + " -m " + dpath + " ";
                                Helper.Log.Debug("启动参数:" + args);
                                System.Diagnostics.Process p = new System.Diagnostics.Process();
                                p.StartInfo.FileName = fullname;
                                p.StartInfo.Arguments = args;
                                p.StartInfo.WorkingDirectory = dpath;
                                p.Start();
                                Helper.Log.Debug("已运行备份程序:" + fullname);
                            }
                        }
                        return;
                    }
                    else
                    {
                        //准备升级
                        Helper.Log.Debug("开始升级更新");
                        await UpdateApp(client, options, needUpdates, bashPath, updatePath, downUpateLibPath, dpath);
                        Helper.Log.Debug("程序更新完毕");
                    }
                }
                else
                {
                    Helper.Log.Debug("无需更新");

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (!options.IsService)
                        {
                            if (!string.IsNullOrWhiteSpace(options.FileName))
                            {
                                var processName = options.FileName.Substring(0, options.FileName.LastIndexOf("."));
                                var plist = System.Diagnostics.Process.GetProcessesByName(processName);
                                if (plist.Length == 0)
                                {
                                    var bashPath = AppDomain.CurrentDomain.BaseDirectory;

                                    var updatePath = options.Path;
                                    if (string.IsNullOrWhiteSpace(updatePath))
                                    {
                                        updatePath = options.UpdateLibName;
                                    }

                                    if (Path.IsPathRooted(updatePath))//是绝对路径,则bashPath设为空串
                                    {
                                        bashPath = "";
                                    }

                                    var fullName = System.IO.Path.Combine(bashPath, updatePath, options.FileName);
                                    if (File.Exists(fullName))
                                    {
                                        System.Diagnostics.Process p = new System.Diagnostics.Process();
                                        p.StartInfo.FileName = fullName;
                                        p.StartInfo.Arguments = options.Args ?? "";
                                        p.StartInfo.WorkingDirectory = System.IO.Path.Combine(bashPath, updatePath);
                                        p.Start();
                                    }
                                }

                            }
                        }
                    }
                }
            });

            try
            {
                //获取本地版本号
                var settingPath = System.IO.Path.Combine(spath, EHelper.VersionFileName);
                VersionModel versionModel = null;
                if (File.Exists(settingPath))
                {
                    versionModel = File.ReadAllText(settingPath).JsonTo<VersionModel>();
                }
                var cver = options.CurrentVersion;
                if (versionModel != null)
                {
                    cver = versionModel.CurrentVersion;
                }

                var info = new AnyeSoft.Common.Service.APIRequest() { ApiPath = ApiPaths.GetAllVersion, AppID = options.User, Data = new ApiVersionModel() { UpdateLibName = options.UpdateLibName, Version = cver }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + options.Key).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                Helper.Log.Error(ex);
            }

            await responseReaderTask;
        }

        private static async Task UpdateApp(gRPC.gRPCClient client, AddOptions options, List<LibVersionInfo> needUpdates, string bashPath, string updatePath, string downUpateLibPath, string dpath, bool isclose = true, bool isbak = true)
        {
            //下载更新到临时目录
            var isDownErr = false;
            var duplib = new DirectoryInfo(System.IO.Path.Combine(downUpateLibPath));
            if (duplib.Parent.Exists)
            {
                duplib.Parent.Delete(true);
            }
            duplib.Create();

            await Task.Run(async () =>
            {
                foreach (var updateConfig in needUpdates)
                {
                    Helper.Log.Debug("开始下载版本:" + updateConfig.UpdateLibName + " => " + updateConfig.Version);
                    Helper.Log.Debug("版本信息:" + updateConfig.VersionText);
                    Helper.Log.Debug("发行说明:" + updateConfig.ReleaseNotes);
                    foreach (var fileVersion in updateConfig.FileVersions)
                    {
                        if (fileVersion.FileType == EnumFileType.File)
                        {
                            if (fileVersion.Action == EnumUpdateAction.FileUpdate)
                            {
                                var chatCall = client.Chat();
                                List<APIReply> contentList = new List<APIReply>();
                                var responseReaderTask = Task.Run(async () =>
                                {
                                    while (await chatCall.ResponseStream.MoveNext())
                                    {
                                        var note = chatCall.ResponseStream.Current;
                                        contentList.Add(note);
                                        Helper.Log.Debug("接收文件:" + fileVersion.FileName + "[" + note.FileBlock + "]");
                                    }

                                    var filepath = System.IO.Path.Combine(downUpateLibPath, fileVersion.FileName);
                                    var filepathInfo = new FileInfo(filepath);
                                    if (!filepathInfo.Directory.Exists)
                                    {
                                        filepathInfo.Directory.Create();
                                    }
                                    FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite);
                                    contentList.OrderBy(x => x.FileBlock).ToList().ForEach(x => x.FileContents.WriteTo(fs));
                                    fs.Close();
                                    Helper.Log.Info("保存文件:" + filepath);
                                    var md5 = filepath.MD5File();
                                    if (md5 == fileVersion.MD5)
                                    {
                                        Helper.Log.Debug("文件校验成功:" + filepath);
                                    }
                                    else
                                    {
                                        isDownErr = true;
                                        Helper.Log.Error("文件校验失败:" + filepath);
                                    }
                                });

                                try
                                {
                                    var info = new APIRequest() { ApiPath = ApiPaths.FileDown, AppID = options.User, Data = new ApiDownFileModel() { UpdateLibName = updateConfig.UpdateLibName, Version = updateConfig.Version, FileName = fileVersion.FileName }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                                    info.Sign = (info.AppID + info.Data + info.Time + options.Key).ToMd5();

                                    await chatCall.RequestStream.WriteAsync(info);
                                    await chatCall.RequestStream.CompleteAsync();
                                }
                                catch (RpcException ex)
                                {
                                    Helper.Log.Error(ex);
                                    isDownErr = true;
                                }

                                await responseReaderTask;
                            }

                        }
                    }
                }
            });

            if (isDownErr)
            {
                return;
            }

            if (isclose)
            {
                //关闭程序或服务
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    await Task.Run(() =>
                    {
                        if (options.IsService)
                        {
                            var name = options.FileName;
                            Helper.Log.Debug("准备停止服务:" + name);
                            var str = EHelper.StopServiceLinux(name);
                            if (string.IsNullOrWhiteSpace(str))
                            {
                                Helper.Log.Debug("服务已停止:" + name);
                            }
                            else
                            {
                                Helper.Log.Warn("服务停止失败:" + str);
                            }

                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(options.FileName))
                            {

                                Helper.Log.Debug("准备关闭程序:" + options.FileName);
                                var pinfos = EHelper.GetPInfos(options.ExecName, options.FileName);
                                foreach (var pinfo in pinfos)
                                {
                                    try
                                    {
                                        var process = System.Diagnostics.Process.GetProcessById(pinfo.PID);
                                        process.Kill();
                                        process.WaitForExit();
                                        Helper.Log.Debug("已关闭程序:" + options.FileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.Log.Error(ex);
                                        return;
                                    }
                                }
                                if (pinfos.Count == 0)
                                {
                                    Helper.Log.Debug("未发现程序运行:" + options.FileName);
                                }
                            }
                        }

                        Task.Delay(1000);
                    });

                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (options.IsService)
                    {
                        var name = options.FileName;
                        var hasService = EHelper.IsWindowsServiceInstalled(name);
                        if (hasService)
                        {
                            Helper.Log.Debug("准备停止服务:" + name);
                            EHelper.StopService(name);
                            Helper.Log.Debug("服务已停止:" + name);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(options.FileName))
                        {

                            var processName = System.IO.Path.GetFileNameWithoutExtension(options.FileName);
                            Helper.Log.Debug("准备关闭程序:" + processName);
                            var plist = System.Diagnostics.Process.GetProcessesByName(processName);
                            foreach (System.Diagnostics.Process thisProc in plist)
                            {
                                try
                                {
                                    if (!thisProc.CloseMainWindow())
                                    {
                                        Helper.Log.Debug("关闭程序失败:" + processName);
                                        thisProc.Kill();
                                    }
                                    thisProc.WaitForExit();
                                    Helper.Log.Debug("已关闭程序:" + processName);
                                }
                                catch (Exception ex)
                                {
                                    Helper.Log.Error(ex);
                                    return;
                                }
                            }
                            if (plist.Length == 0)
                            {
                                Helper.Log.Debug("未发现程序运行:" + processName);
                            }
                        }
                    }
                }
            }


            var spath = new DirectoryInfo(System.IO.Path.Combine(bashPath, updatePath)).FullName;//源

            if (isbak)
            {
                //将应用备份到备份目录
                if (System.IO.Directory.Exists(spath))
                {
                    if (Directory.Exists(dpath))
                    {
                        Helper.Log.Debug("清理之前备份:" + dpath);
                        Directory.Delete(dpath, true);
                        Helper.Log.Debug("清理备份成功:" + dpath);
                    }

                    Helper.Log.Debug("开始备份:" + spath);
                    EHelper.DirectoryCopy(spath, dpath);
                    Helper.Log.Debug("备份成功:" + dpath);
                }
            }


            //更新
            UpdateFileForAddOptions(bashPath, updatePath, downUpateLibPath, dpath, needUpdates);


            //启动程序或服务
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (options.IsService)
                {
                    Helper.Log.Debug("准备启动服务:" + options.FileName);
                    var str = EHelper.StartServiceLinux(options.FileName);
                    if (string.IsNullOrWhiteSpace(str))
                    {
                        Helper.Log.Debug("服务已启动:" + options.FileName);
                    }
                    else
                    {
                        Helper.Log.Warn("服务启动失败:" + str);
                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(options.FileName))
                    {
                        var fullname = Path.Combine(spath, options.FileName);
                        if (File.Exists(fullname))
                        {
                            Helper.Log.Debug("准备启动程序:" + fullname + " " + options.Args ?? "");
                            EHelper.StartClient(options.ExecName, fullname, options.Args ?? "");
                            Helper.Log.Debug("程序已启动:" + fullname + " " + options.Args ?? "");
                        }
                    }

                }

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (options.IsService)
                {
                    var name = options.FileName;
                    var hasService = EHelper.IsWindowsServiceInstalled(name);
                    if (hasService)
                    {
                        Helper.Log.Debug("准备启动服务:" + name);
                        EHelper.ReStartService(name);
                        Helper.Log.Debug("服务已启动:" + name);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(options.FileName))
                    {
                        var processName = options.FileName;
                        var fullName = System.IO.Path.Combine(spath, processName);
                        if (File.Exists(fullName))
                        {
                            Helper.Log.Debug("准备运行程序:" + fullName + " " + options.Args ?? "");
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = fullName;
                            p.StartInfo.Arguments = options.Args ?? "";
                            p.StartInfo.WorkingDirectory = spath;
                            p.Start();
                            Helper.Log.Debug("已运行程序:" + fullName + " " + options.Args ?? "");
                        }
                    }

                }
            }

        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="bashPath">更新root目录</param>
        /// <param name="updatePath">更新程序路径</param>
        /// <param name="downUpateLibPath">下载程序路径</param>
        /// <param name="dpath">目标目录</param>
        /// <param name="needUpdates">需要更新的信息</param>
        static void UpdateFileForAddOptions(string bashPath, string updatePath, string downUpateLibPath, string dpath, List<LibVersionInfo> needUpdates)
        {
            var isUpdateErr = false;
            var spath = new DirectoryInfo(System.IO.Path.Combine(bashPath, updatePath)).FullName;
            foreach (var updateConfig in needUpdates)
            {
                Helper.Log.Debug("开始更新版本:" + updateConfig.UpdateLibName + " => " + updateConfig.Version);
                Helper.Log.Debug("版本信息:" + updateConfig.VersionText);
                Helper.Log.Debug("发行说明:" + updateConfig.ReleaseNotes);
                foreach (var fileVersion in updateConfig.FileVersions)
                {
                    var sfilepath = System.IO.Path.Combine(downUpateLibPath, fileVersion.FileName);
                    var dfilepath = System.IO.Path.Combine(spath, fileVersion.FileName);

                    if (fileVersion.FileType == EnumFileType.File)
                    {
                        if (fileVersion.Action == EnumUpdateAction.FileUpdate)
                        {
                            var filepathInfo = new FileInfo(dfilepath);
                            if (!filepathInfo.Directory.Exists)
                            {
                                filepathInfo.Directory.Create();
                            }
                            System.IO.File.Copy(sfilepath, dfilepath, true);
                            Helper.Log.Info("更新文件:" + dfilepath);
                        }
                        else if (fileVersion.Action == EnumUpdateAction.FileDelete)
                        {
                            if (File.Exists(dfilepath))
                            {
                                File.Delete(dfilepath);
                                Helper.Log.Info("删除文件:" + dfilepath);
                            }
                        }
                    }
                    else if (fileVersion.FileType == EnumFileType.Directory)
                    {
                        if (fileVersion.Action == EnumUpdateAction.FileUpdate)
                        {
                            if (!Directory.Exists(dfilepath))
                            {
                                Directory.CreateDirectory(dfilepath);
                            }
                        }
                        else if (fileVersion.Action == EnumUpdateAction.FileDelete)
                        {
                            if (Directory.Exists(dfilepath))
                            {
                                Directory.Delete(dfilepath, true);
                            }
                        }

                    }

                }

                var versionModel = new VersionModel() { UpdateLibName = updateConfig.UpdateLibName, CurrentVersion = updateConfig.Version, VersionText = updateConfig.VersionText, ReleaseNotes = updateConfig.ReleaseNotes, UpdateDateTime = DateTime.Now };
                var settingPath = System.IO.Path.Combine(spath, EHelper.VersionFileName);
                File.WriteAllText(settingPath, versionModel.ToJson(true));
                Helper.Log.Debug("更新本地版本配置文件");
            }

            if (isUpdateErr)
            {
                //还原备份
                if (System.IO.Directory.Exists(dpath))
                {
                    Helper.Log.Debug("开始还原备份");
                    System.IO.Directory.Delete(spath, true);
                    EHelper.DirectoryCopy(dpath, spath);
                    Helper.Log.Debug("还原备份成功");
                }
            }
            else
            {
                //删除临时下载目录
                var dir = new DirectoryInfo(downUpateLibPath);
                if (dir.Parent != null)
                {
                    System.IO.Directory.Delete(dir.Parent.FullName, true);
                }

            }
        }



        private static void Get(GetOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.Name))
            {
                var t = EHelper.Data.FirstOrDefault(m => m.Name == options.Name);
                if (t != null)
                {
                    Console.WriteLine(t.ToJson(options.IsFormatting));
                }
            }
            else
            {
                Console.WriteLine(EHelper.Data.ConvertAll(m => m.Name).ToJson(options.IsFormatting));
            }

        }

        private static void Del(DelOptions options)
        {
            EHelper.Data.RemoveAll(m => m.Name == options.Name);
            EHelper.SaveData();
        }

        private static void Edit(EditOptions options)
        {
            var t = EHelper.Data.FirstOrDefault(m => m.Name == options.Name);
            if (t == null)
            {
                Console.WriteLine("更新库配置:" + options.Name + ",不存在");
                return;
            }
            t.CurrentVersion = options.CurrentVersion ?? t.CurrentVersion;
            t.FileName = options.FileName ?? t.FileName;
            t.IsService = options.IsService ?? t.IsService;
            t.Key = options.Key ?? t.Key;
            t.Path = options.Path ?? t.Path;
            t.Target = options.Target ?? t.Target;
            t.UpdateLibName = options.UpdateLibName ?? t.UpdateLibName;
            t.User = options.User ?? t.User;
            EHelper.SaveData();
        }
        private static void Add(AddOptions options)
        {
            var t = EHelper.Data.FirstOrDefault(m => m.Name == options.Name);
            if (t != null)
            {
                Console.WriteLine("更新库配置:" + options.Name + ",已存在");
                return;
            }
            EHelper.Data.Add(options);
            EHelper.SaveData();
        }


        private static void DisplayHelp(ParserResult<object> parserResult, IEnumerable<Error> errs)
        {
            var p = System.Reflection.Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(m => m.AttributeType == typeof(System.Reflection.AssemblyProductAttribute));
            string title = p.ConstructorArguments.FirstOrDefault().Value?.ToString();
            var helpText = HelpText.AutoBuild(parserResult, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = title;
                return h;
            }, e => e);
            Console.WriteLine(helpText);
        }


        private static void Default(DefaultOptions options)
        {
            var p = System.Reflection.Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(m => m.AttributeType == typeof(System.Reflection.AssemblyProductAttribute));
            string title = p.ConstructorArguments.FirstOrDefault().Value?.ToString();
            if (options.ShowVersion)
            {
                Console.WriteLine(title);
                return;
            }
            Console.WriteLine("========================================================");
            Console.WriteLine(title);
            Console.WriteLine("========================================================");
            Console.WriteLine("add           新增更新库配置命令");
            Console.WriteLine("del           删除更新库配置命令");
            Console.WriteLine("edit          编辑更新库配置命令");
            Console.WriteLine("get           查看更新库配置命令");
            Console.WriteLine("update        执行更新库配置命令");
            Console.WriteLine("run           直接执行更新命令");
            Console.WriteLine("updself       自我更新命令");
            Console.WriteLine("default       默认命令");

            Console.WriteLine("  -v, --ver    版本信息");
            Console.WriteLine("  --help       显示命令帮助信息");
            Console.WriteLine("  --version    显示版本信息");
            Console.WriteLine("");
        }

    }
}
