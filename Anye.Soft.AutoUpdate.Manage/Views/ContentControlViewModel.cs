using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using Grpc.Core;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    public class ContentControlViewModel : Screen
    {
        public string ActionName { get; set; } = "";
        public string ActionInfo { get; set; } = "";
        public int ActionCount { get; set; } = 100;
        public int ActionCurIndex { get; set; } = 0;

        public int BlockActionCount { get; set; } = 10;
        public int BlockActionCurIndex { get; set; } = 0;

        public bool IsCompleted { get; set; } = true;

        private IWindowManager windowManager;
        public Channel Channel { get; set; }
        public gRPC.gRPCClient RPCClient { get; set; }
        public bool IsLink { get; set; } = false;
        public ManageDataModel DataModel { get; set; }

        public UpdateLibUI SelectUpdateLib { get; set; }
        public string LinkInfo { get; set; }

        public ContentControlViewModel(IWindowManager windowManager, ManageDataModel manageData)
        {

            this.windowManager = windowManager;

            DataModel = manageData;
            if (DataModel == null) { throw new Exception("manageData不能为null"); }
            try
            {
                Channel = new Channel(DataModel.IPort, ChannelCredentials.Insecure);
                RPCClient = new gRPC.gRPCClient(Channel);
                Task.Run(async () =>
                {
                    await GetLibs();
                });
            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
                this.windowManager.ShowDialog(new MessageBoxViewModel("连接错误", ex.Message));
            }
        }

        private async Task GetLibs()
        {
            Execute.OnUIThreadSync(() =>
            {
                DataModel.Libs.Clear();
            });

            var chatCall = RPCClient.Chat();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        //操作成功
                        Execute.OnUIThreadSync(() =>
                        {
                            IsLink = true;
                            LinkInfo = note.Msg;

                            var list = note.Data.JsonTo<ObservableCollection<UpdateLibUI>>();
                            foreach (var item in list)
                            {
                                DataModel.Libs.Add(item);
                            }

                        });

                    }

                }

            });

            try
            {
                var info = new APIRequest() { ApiPath = ApiPaths.ManageGetlibs, AppID = DataModel.AdminName, Data = "", Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                IsLink = false;
                LinkInfo = "连接失败";
            }

            await responseReaderTask;
        }

        public void ShowAddUpdateLibView()
        {
            var viewModel = new AddUpdateLibViewModel(RPCClient, DataModel);
            bool? result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                if (!viewModel.ErrorInfo.IsNullOrEmptyOrSpace())
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel(viewModel.ErrorInfo));
                    return;
                }
                Task.Run(async () =>
                {
                    await GetLibs();
                });
            }
        }

        public async void ShowDelUpdateLib()
        {
            if (SelectUpdateLib == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择更新库"));
                return;
            }
            var rz = this.windowManager.ShowDialog(new MessageBoxViewModel("确定要删除:[" + SelectUpdateLib.Name + "]?", true));
            if (rz == true)
            {
                await DelLib();
            }
        }

        public async Task DelLib()
        {
            var chatCall = RPCClient.Chat();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        //操作成功
                        await GetLibs();
                    }
                    else
                    {
                        Execute.OnUIThreadSync(() =>
                        {
                            this.windowManager.ShowDialog(new MessageBoxViewModel(note.Msg));
                        });
                    }
                }

            });

            try
            {
                var info = new APIRequest() { ApiPath = ApiPaths.ManageDellib, AppID = DataModel.AdminName, Data = new ApiVersionModel() { UpdateLibName = SelectUpdateLib.Name }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                Execute.OnUIThreadSync(() =>
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel("连接失败"));
                });
            }

            await responseReaderTask;
        }

        public void AddVerLib()
        {
            if (SelectUpdateLib == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择更新库"));
                return;
            }


            var viewModel = new AddVersionLibViewModel(RPCClient, DataModel, SelectUpdateLib);

            bool? result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                if (!viewModel.ErrorInfo.IsNullOrEmptyOrSpace())
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel(viewModel.ErrorInfo));
                    return;
                }

            }
        }

        public void SettingVerLib(LibVersionInfoUI libVersion)
        {
            if (SelectUpdateLib == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择更新库"));
                return;
            }
            if (libVersion == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择版本库"));
                return;
            }

            var viewModel = new AddVersionLibViewModel(RPCClient, DataModel, SelectUpdateLib, true);

            bool? result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                if (!viewModel.ErrorInfo.IsNullOrEmptyOrSpace())
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel(viewModel.ErrorInfo));
                    return;
                }

            }
        }

        public async Task DelVerLib(LibVersionInfoUI libVersion)
        {
            if (SelectUpdateLib == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择更新库"));
                return;
            }
            if (libVersion == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择版本库"));
                return;
            }
            if (libVersion.IsRelease || (libVersion.IsEnablePreReleaseTime && DateTime.Now >= libVersion.PreReleaseTime))
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("版本已发布,无法删除"));
                return;
            }

            var rz = this.windowManager.ShowDialog(new MessageBoxViewModel("确定要删除:[" + libVersion.Version + "]版本?", true));

            if (rz != true)
            {
                return;
            }

            var chatCall = RPCClient.Chat();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        //操作成功
                        Execute.OnUIThreadSync(() =>
                        {
                            SelectUpdateLib.Configs.Remove(libVersion);
                        });

                    }
                    else
                    {
                        Execute.OnUIThreadSync(() =>
                        {
                            this.windowManager.ShowDialog(new MessageBoxViewModel(note.Msg));
                        });
                    }
                }

            });

            try
            {
                var info = new APIRequest() { ApiPath = ApiPaths.ManageDelver, AppID = DataModel.AdminName, Data = new ApiVersionModel() { UpdateLibName = SelectUpdateLib.Name, Version = libVersion.Version }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                Execute.OnUIThreadSync(() =>
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel("连接失败"));
                });
            }

            await responseReaderTask;
        }

        public async void AddDirFiles()
        {
            if (SelectUpdateLib.LibVersionInfoUI == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择版本库"));
                return;
            }
            if (SelectUpdateLib.LibVersionInfoUI.IsRelease || (SelectUpdateLib.LibVersionInfoUI.IsEnablePreReleaseTime && DateTime.Now >= SelectUpdateLib.LibVersionInfoUI.PreReleaseTime))
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("版本已发布,无法操作"));
                return;
            }

            var settings = new FolderBrowserDialogSettings();
            var folderBrowser = new DefaultFrameworkDialogFactory().CreateFolderBrowserDialog(settings);
            var win = (this.Parent as IScreen).View as System.Windows.Window;
            bool? success = folderBrowser.ShowDialog(win);
            if (success == true)
            {
                List<FileVersionInfo> fileVersions = new();
                string basePath = settings.SelectedPath;

                var files = Directory.GetFiles(settings.SelectedPath, "*", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var filename = fileInfo.FullName.Replace(System.IO.Path.Combine(basePath + "\\"), "").Replace("\\","/");
                    var fver = new FileVersionInfo() { FileName = filename, FileSize = fileInfo.Length, MD5 = file.MD5File(), FileType = EnumFileType.File, Action = EnumUpdateAction.FileUpdate };
                    fileVersions.Add(fver);
                }

                var dirs = Directory.GetDirectories(settings.SelectedPath, "*", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    if (dirInfo.GetFiles().Length == 0 && dirInfo.GetDirectories().Length == 0)
                    {
                        var filename = dirInfo.FullName.Replace(System.IO.Path.Combine(basePath + "\\"), "").Replace("\\", "/");
                        var fver = new FileVersionInfo() { FileName = filename, FileSize = 0, MD5 = "", FileType = EnumFileType.Directory, Action = EnumUpdateAction.FileUpdate };
                        fileVersions.Add(fver);
                    }
                }

                if (fileVersions.Count > 0)
                {
                    //标记上一版本相同文件和被删除的文件
                    var last = SelectUpdateLib.GetLastReleaseLibVersionInfoUI();
                    if (last != null)
                    {
                        foreach (var lastfileVersion in last.FileVersions)
                        {
                            var xt = fileVersions.FirstOrDefault(m => m.FileName == lastfileVersion.FileName);
                            if (xt != null && xt.MD5 == lastfileVersion.MD5)
                            {
                                xt.Action = EnumUpdateAction.None;
                            }
                            else
                            {
                                var delfile = new FileVersionInfo();
                                delfile.Action = EnumUpdateAction.FileDelete;
                                delfile.FileName = lastfileVersion.FileName;
                                delfile.FileSize = 0;
                                delfile.FileType = lastfileVersion.FileType;
                                fileVersions.Insert(0, delfile);
                            }
                        }
                    }

                    await UpFiles(basePath, fileVersions);
                }
            }

        }

        public async void AddFiles()
        {
            if (SelectUpdateLib.LibVersionInfoUI == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择版本库"));
                return;
            }
            if (SelectUpdateLib.LibVersionInfoUI.IsRelease || (SelectUpdateLib.LibVersionInfoUI.IsEnablePreReleaseTime && DateTime.Now >= SelectUpdateLib.LibVersionInfoUI.PreReleaseTime))
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("版本已发布,无法操作"));
                return;
            }
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                if (ofd.FileNames != null && ofd.FileNames.Length > 0)
                {
                    List<FileVersionInfo> fileVersions = new();
                    string basePath = new FileInfo(ofd.FileNames[0]).DirectoryName;

                    foreach (var file in ofd.FileNames)
                    {
                        var fileInfo = new FileInfo(file);
                        var filename = fileInfo.FullName.Replace(System.IO.Path.Combine(basePath + "\\"), "").Replace("\\", "/");
                        var fver = new FileVersionInfo() { FileName = filename, FileSize = fileInfo.Length, MD5 = file.MD5File(), FileType = EnumFileType.File, Action = EnumUpdateAction.FileUpdate };

                        fileVersions.Add(fver);
                    }

                    if (fileVersions.Count > 0)
                    {
                        //标记上一版本相同文件和被删除的文件
                        var last = SelectUpdateLib.GetLastReleaseLibVersionInfoUI();
                        if (last != null)
                        {
                            foreach (var lastfileVersion in last.FileVersions)
                            {
                                var xt = fileVersions.FirstOrDefault(m => m.FileName == lastfileVersion.FileName);
                                if (xt != null && xt.MD5 == lastfileVersion.MD5)
                                {
                                    xt.Action = EnumUpdateAction.None;
                                }
                                else
                                {
                                    var delfile = new FileVersionInfo();
                                    delfile.Action = EnumUpdateAction.FileDelete;
                                    delfile.FileName = lastfileVersion.FileName;
                                    delfile.FileSize = 0;
                                    delfile.FileType = lastfileVersion.FileType;
                                    fileVersions.Insert(0, delfile);
                                }
                            }
                        }

                        await UpFiles(basePath, fileVersions);
                    }
                }
            }
        }

        async Task UpFiles(string bashPath, List<FileVersionInfo> fileVersions)
        {
            //开始上传文件
            Execute.OnUIThreadSync(() =>
            {
                ActionCount = fileVersions.Count;
                ActionName = "上传文件:";
                IsCompleted = false;
                ActionCurIndex = 0;
                BlockActionCurIndex = 0;
            });

            var chatCall = RPCClient.Chat();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await chatCall.ResponseStream.MoveNext())
                {
                    var note = chatCall.ResponseStream.Current;
                    if (note.Code == 0)
                    {
                        //操作成功
                        Execute.OnUIThreadSync(() =>
                        {
                            BlockActionCurIndex = BlockActionCount;
                            ActionCurIndex++;

                            var fver = note.Data.JsonTo<FileVersionInfo>();
                            if (fver != null)
                            {
                                ActionInfo = fver.FileName;
                                var ver = SelectUpdateLib.LibVersionInfoUI.FileVersions.FirstOrDefault(m => m.FileName == fver.FileName);
                                if (ver == null)
                                {
                                    SelectUpdateLib.LibVersionInfoUI.FileVersions.Insert(0, fver);
                                }
                                else
                                {
                                    SelectUpdateLib.LibVersionInfoUI.FileVersions.Remove(ver);
                                    SelectUpdateLib.LibVersionInfoUI.FileVersions.Insert(0, fver);
                                }
                            }
                        });
                    }
                    else if (note.Code == 1)
                    {
                        //文件上传中
                        Execute.OnUIThreadSync(() =>
                        {
                            var fver = note.Data.JsonTo<FileVersionInfo>();
                            if (fver != null)
                            {
                                BlockActionCurIndex++;
                                ActionInfo = fver.FileName + "[" + BlockActionCurIndex + "]";
                            }

                        });
                    }
                    else
                    {
                        Execute.OnUIThreadSync(() =>
                        {
                            ActionName = "上传失败:" + note.Msg;
                        });
                    }
                }
                Execute.OnUIThreadSync(() =>
                {
                    BlockActionCurIndex = BlockActionCount;
                    ActionName = "上传完成";
                    ActionInfo = "";
                    ActionCurIndex = ActionCount;
                    IsCompleted = true;
                });

            });

            foreach (var fver in fileVersions)
            {
                try
                {
                    if (fver.FileType == EnumFileType.File)
                    {
                        byte[] byteArray = Array.Empty<byte>();
                        if (fver.Action != EnumUpdateAction.FileDelete)
                        {
                            byteArray = File.ReadAllBytes(System.IO.Path.Combine(bashPath, fver.FileName));
                        }
                        int btSize = byteArray.Length;
                        int buffSize = 1024 * 1024; //1M
                        int lastBiteSize = btSize % buffSize;
                        int currentTimes = 0;
                        int loopTimes = btSize / buffSize;

                        Execute.OnUIThreadSync(() =>
                        {
                            BlockActionCount = loopTimes + 1;
                            BlockActionCurIndex = 0;
                        });

                        while (currentTimes <= loopTimes)
                        {
                            Google.Protobuf.ByteString sbytes;
                            if (currentTimes == loopTimes)//最后一个
                            {
                                sbytes = Google.Protobuf.ByteString.CopyFrom(byteArray, currentTimes * buffSize, lastBiteSize);
                            }
                            else
                            {
                                sbytes = Google.Protobuf.ByteString.CopyFrom(byteArray, currentTimes * buffSize, buffSize);
                            }

                            var apiUpFileModel = new ApiUpFileModel() { UpdateLibName = SelectUpdateLib.Name, Version = SelectUpdateLib.LibVersionInfoUI.Version, FileVersion = fver };
                            var info = new APIRequest() { ApiPath = ApiPaths.FileUp, AppID = DataModel.AdminName, Data = apiUpFileModel.ToJson(), Time = DateTime.Now.ToTimestamp(), FileBlock = currentTimes, FileBlockLastIndex = loopTimes, FileContents = sbytes };
                            info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();


                            Execute.OnUIThreadSync(() =>
                            {
                                ActionInfo = fver.FileName;
                            });

                            await chatCall.RequestStream.WriteAsync(info);

                            currentTimes++;
                        }
                    }
                    else if (fver.FileType == EnumFileType.Directory)
                    {
                        Execute.OnUIThreadSync(() =>
                        {
                            BlockActionCount = 1;
                            BlockActionCurIndex = 0;
                            ActionInfo = fver.FileName;
                        });

                        var apiUpFileModel = new ApiUpFileModel() { UpdateLibName = SelectUpdateLib.Name, Version = SelectUpdateLib.LibVersionInfoUI.Version, FileVersion = fver };
                        var info = new APIRequest() { ApiPath = ApiPaths.FileUp, AppID = DataModel.AdminName, Data = apiUpFileModel.ToJson(), Time = DateTime.Now.ToTimestamp() };
                        info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                        await chatCall.RequestStream.WriteAsync(info);

                    }

                }
                catch (RpcException ex)
                {
                    Execute.OnUIThreadSync(() =>
                    {
                        ActionInfo = ex.Message;
                    });
                }

            }

            await chatCall.RequestStream.CompleteAsync();
        }

        public async Task DelFile(FileVersionInfo fileVersionInfo)
        {

            if (SelectUpdateLib == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择更新库"));
                return;
            }
            if (SelectUpdateLib.LibVersionInfoUI == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择版本库"));
                return;
            }
            if (fileVersionInfo == null)
            {
                this.windowManager.ShowDialog(new MessageBoxViewModel("请选择文件"));
                return;
            }

            var rz= this.windowManager.ShowDialog(new MessageBoxViewModel("确定要删除:[" + fileVersionInfo.FileName + "]?",true));
            if (rz == true)
            {

                var chatCall = RPCClient.Chat();

                var responseReaderTask = Task.Run(async () =>
                {
                    while (await chatCall.ResponseStream.MoveNext())
                    {
                        var note = chatCall.ResponseStream.Current;
                        if (note.Code == 0)
                        {
                            //操作成功
                            Execute.OnUIThreadSync(() =>
                                {
                                    SelectUpdateLib.LibVersionInfoUI.FileVersions.Remove(fileVersionInfo);
                                });

                        }
                        else
                        {
                            Execute.OnUIThreadSync(() =>
                            {
                                this.windowManager.ShowDialog(new MessageBoxViewModel(note.Msg));
                            });
                        }
                    }

                });

                try
                {
                    var info = new APIRequest() { ApiPath = ApiPaths.ManageDelfile, AppID = DataModel.AdminName, Data = new ApiVersionModel() { UpdateLibName = SelectUpdateLib.Name, Version = SelectUpdateLib.LibVersionInfoUI.Version, FileName = fileVersionInfo.FileName }.ToJson(), Time = DateTime.Now.ToTimestamp() };
                    info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                    await chatCall.RequestStream.WriteAsync(info);
                    await chatCall.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    Execute.OnUIThreadSync(() =>
                    {
                        this.windowManager.ShowDialog(new MessageBoxViewModel("连接失败", ex.Message));
                    });
                }

                await responseReaderTask;
            }
        }

        public void PublishApp()
        {
            Execute.OnUIThreadSync(async () =>
            {
                var rz= this.windowManager.ShowDialog(new MessageBoxViewModel("确定要发布:[" + SelectUpdateLib.LibVersionInfoUI.Version + "]版本?",true));
                if (rz != true)
                {
                    return;
                }
                if (SelectUpdateLib.LibVersionInfoUI.IsRelease)
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel("该版本已经发布"));
                    return;
                }

                SelectUpdateLib.LibVersionInfoUI.IsRelease = true;
                var chatCall = RPCClient.Chat();

                var responseReaderTask = Task.Run(async () =>
                {
                    while (await chatCall.ResponseStream.MoveNext())
                    {
                        var note = chatCall.ResponseStream.Current;
                        if (note.Code != 0)
                        {
                            SelectUpdateLib.LibVersionInfoUI.IsRelease = false;
                            Execute.OnUIThreadSync(() =>
                            {
                                this.windowManager.ShowDialog(new MessageBoxViewModel(note.Msg));
                            });
                        }


                    }

                });

                try
                {
                    var info = new APIRequest() { ApiPath = ApiPaths.ManageUpdver, AppID = DataModel.AdminName, Data = SelectUpdateLib.LibVersionInfoUI.ToJson(), Time = DateTime.Now.ToTimestamp(), };
                    info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                    await chatCall.RequestStream.WriteAsync(info);
                    await chatCall.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    SelectUpdateLib.LibVersionInfoUI.IsRelease = false;
                    Execute.OnUIThreadSync(() =>
                    {
                        this.windowManager.ShowDialog(new MessageBoxViewModel("连接失败"));
                    });
                }

                await responseReaderTask;

            });

        }

        public void UnPublishApp()
        {
            Execute.OnUIThreadSync(async () =>
            {
                var rz= this.windowManager.ShowDialog(new MessageBoxViewModel("确定要取消发布:[" + SelectUpdateLib.LibVersionInfoUI.Version + "]版本?",true));
                if (rz != true)
                {
                    return;
                }
                if (!SelectUpdateLib.LibVersionInfoUI.IsRelease)
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel("该版本已经取消发布"));
                    return;
                }
                var lastlibver = SelectUpdateLib.GetLastReleaseLibVersionInfoUI();
                if (lastlibver != null && lastlibver.Version != SelectUpdateLib.LibVersionInfoUI.Version)
                {
                    this.windowManager.ShowDialog(new MessageBoxViewModel("该版本不是最后发布版本,无法取消"));
                    return;
                }

                SelectUpdateLib.LibVersionInfoUI.IsRelease = false;
                var chatCall = RPCClient.Chat();

                var responseReaderTask = Task.Run(async () =>
                {
                    while (await chatCall.ResponseStream.MoveNext())
                    {
                        var note = chatCall.ResponseStream.Current;
                        if (note.Code != 0)
                        {
                            SelectUpdateLib.LibVersionInfoUI.IsRelease = true;
                            Execute.OnUIThreadSync(() =>
                            {
                                this.windowManager.ShowDialog(new MessageBoxViewModel(note.Msg));
                            });
                        }


                    }

                });

                try
                {
                    var info = new APIRequest() { ApiPath = ApiPaths.ManageUpdver, AppID = DataModel.AdminName, Data = SelectUpdateLib.LibVersionInfoUI.ToJson(), Time = DateTime.Now.ToTimestamp(), };
                    info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                    await chatCall.RequestStream.WriteAsync(info);
                    await chatCall.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    SelectUpdateLib.LibVersionInfoUI.IsRelease = true;
                    Execute.OnUIThreadSync(() =>
                    {
                        this.windowManager.ShowDialog(new MessageBoxViewModel("连接失败",ex.Message));
                    });
                }

                await responseReaderTask;

            });

        }
    }
}
