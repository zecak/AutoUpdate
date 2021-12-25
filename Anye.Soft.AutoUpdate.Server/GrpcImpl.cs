using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Server
{
    public class GrpcImpl : gRPC.gRPCBase
    {
        public static object lockobj_Libs = new object();
        public bool CheckSign(APIRequest request)
        {
            var serverKey = "";
            if (Helper.ServerSetting.Admin.Name == request.AppID)
            {
                serverKey = Helper.ServerSetting.Admin.Pass;
            }
            else if (Helper.ServerSetting.Updater.Name == request.AppID)
            {
                serverKey = Helper.ServerSetting.Updater.Pass;
            }
            var sign = (request.AppID + request.Data + request.Time + serverKey).ToMd5();
            if (sign != request.Sign)
            {
                return false;
            }
            return true;
        }

        public override Task<APIReply> Exec(APIRequest request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new APIReply { Code = (int)ErrorMsg.未知操作, Msg = ErrorMsg.未知操作.ToString() });
            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = (int)ErrorMsg.未知操作, Msg = ErrorMsg.未知操作.ToString() });
            }
        }

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {
            List<APIRequest> contentList = new List<APIRequest>();

            while (await requestStream.MoveNext())
            {
                try
                {
                    var request = requestStream.Current;

                    if (!CheckSign(request))
                    {
                        Helper.Log.Debug("电子签名不一致:" + request.ToJson());
                        APIReply resp = new APIReply() { };
                        resp.Code = (int)ErrorMsg.电子签名不一致;
                        resp.Msg = ErrorMsg.电子签名不一致.ToString();
                        await responseStream.WriteAsync(resp);
                        return;
                    }
                    if (Helper.ServerSetting.Updater.Name == request.AppID && request.ApiPath != ApiPaths.GetAllVersion && request.ApiPath != ApiPaths.FileDown)
                    {
                        Helper.Log.Debug("无权限:" + request.ToJson());
                        APIReply resp = new APIReply() { };
                        resp.Code = (int)ErrorMsg.无权限;
                        resp.Msg = ErrorMsg.无权限.ToString();
                        await responseStream.WriteAsync(resp);
                        return;
                    }
                    try
                    {
                        switch (request.ApiPath)
                        {
                            case ApiPaths.GetAllVersion:
                                {
                                    var vmodel = request.Data.JsonTo<ApiVersionModel>();
                                    if (vmodel == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.请求参数必填, Msg = ErrorMsg.请求参数必填.ToString() });
                                        return;
                                    }
                                    if (vmodel.UpdateLibName.IsNullOrEmptyOrSpace())
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.请求参数更新库名称必填, Msg = ErrorMsg.请求参数更新库名称必填.ToString() });
                                        return;
                                    }
                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == vmodel.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }

                                    var clientUpdInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, lib.Name);
                                    if (!Directory.Exists(clientUpdInfoPath))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库不存在, Msg = ErrorMsg.更新库不存在.ToString() });
                                        return;
                                    }

                                    var needUpdateVersions = lib.GetReleaseVersions(vmodel.Version);

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "版本更新", Data = needUpdateVersions.ToJson() });
                                }
                                break;
                            case ApiPaths.GetVersion:
                                {
                                    var vmodel = request.Data.JsonTo<ApiVersionModel>();
                                    if (vmodel == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.请求参数必填, Msg = ErrorMsg.请求参数必填.ToString() });
                                        return;
                                    }
                                    if (vmodel.UpdateLibName.IsNullOrEmptyOrSpace())
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.请求参数更新库名称必填, Msg = ErrorMsg.请求参数更新库名称必填.ToString() });
                                        return;
                                    }
                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == vmodel.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }

                                    var clientUpdInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, lib.Name);
                                    if (!Directory.Exists(clientUpdInfoPath))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库不存在, Msg = ErrorMsg.更新库不存在.ToString() });
                                        return;
                                    }

                                    var needUpdateVersion = lib.GetReleaseVersion(vmodel.Version);

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "版本更新", Data = needUpdateVersion.ToJson() });
                                }
                                break;
                            case ApiPaths.FileDown:
                                {
                                    var downFileModel = request.Data.JsonTo<ApiDownFileModel>();
                                    var fullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, downFileModel.UpdateLibName, downFileModel.Version.ToString(), downFileModel.FileName);
                                    if (File.Exists(fullName))
                                    {
                                        byte[] byteArray = File.ReadAllBytes(fullName);
                                        int btSize = byteArray.Length;
                                        int buffSize = 1024 * 1024; //1M
                                        int lastBiteSize = btSize % buffSize;
                                        int currentTimes = 0;
                                        int loopTimes = btSize / buffSize;

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
                                            await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "下载文件:" + downFileModel.FileName + "[" + currentTimes + "/" + loopTimes + "][" + sbytes.Count() + "]", FileBlock = currentTimes, FileBlockLastIndex = loopTimes, FileContents = sbytes });
                                            currentTimes++;
                                        }
                                    }
                                }
                                break;
                            case ApiPaths.FileUp:
                                {
                                    //Helper.Log.Debug(request.ApiPath + ":" + request.ToJson());
                                    var apiUpFileModel = request.Data.JsonTo<ApiUpFileModel>();
                                    if (apiUpFileModel == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -920, Msg = "转换参数失败" });
                                        return;
                                    }
                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == apiUpFileModel.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -921, Msg = "更新库信息不存在" });
                                        return;
                                    }
                                    var ver = lib.Configs.FirstOrDefault(m => m.Version == apiUpFileModel.Version);
                                    if (ver == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -922, Msg = "版本库信息不存在" });
                                        return;
                                    }
                                    if (ver.IsRelease || (ver.IsEnablePreReleaseTime && DateTime.Now >= ver.PreReleaseTime))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -314, Msg = "版本已发布,无法操作" });
                                        return;
                                    }

                                    if (apiUpFileModel.FileVersion.Action == EnumUpdateAction.FileDelete)
                                    {
                                        lock (lockobj_Libs)
                                        {
                                            var temp_ver = ver.FileVersions.FirstOrDefault(m => m.FileName == apiUpFileModel.FileVersion.FileName);
                                            if (temp_ver == null)
                                            {
                                                ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                            }
                                            else
                                            {
                                                ver.FileVersions.Remove(temp_ver);
                                                ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                            }
                                            HelperServer.SaveLibsData();
                                        }
                                        await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "添加删除文件标记成功", Data = apiUpFileModel.FileVersion.ToJson() });

                                    }
                                    else
                                    {
                                        if (apiUpFileModel.FileVersion.FileType == EnumFileType.File)
                                        {
                                            contentList.Add(request);
                                            Helper.Log.Debug("上传文件:" + apiUpFileModel.FileVersion.FileName + "[" + request.FileBlock + "/" + request.FileBlockLastIndex + "]" + "[" + request.FileContents.Count() + "]");
                                            if (request.FileBlockLastIndex == request.FileBlock)
                                            {
                                                var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, apiUpFileModel.UpdateLibName, apiUpFileModel.Version.ToString(), apiUpFileModel.FileVersion.FileName);
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
                                                if (md5 == apiUpFileModel.FileVersion.MD5)
                                                {
                                                    Helper.Log.Debug("文件校验成功:" + filepath);
                                                }
                                                else
                                                {
                                                    Helper.Log.Warn("文件校验失败:" + filepath);
                                                    await responseStream.WriteAsync(new APIReply { Code = -999, Msg = "文件校验失败", Data = apiUpFileModel.FileVersion.ToJson() });
                                                    return;
                                                }
                                                lock (lockobj_Libs)
                                                {
                                                    var temp_ver = ver.FileVersions.FirstOrDefault(m => m.FileName == apiUpFileModel.FileVersion.FileName);
                                                    if (temp_ver == null)
                                                    {
                                                        ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                                    }
                                                    else
                                                    {
                                                        ver.FileVersions.Remove(temp_ver);
                                                        ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                                    }
                                                    HelperServer.SaveLibsData();
                                                }
                                                contentList.Clear();

                                                await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "上传文件成功", Data = apiUpFileModel.FileVersion.ToJson() });
                                            }
                                            else
                                            {
                                                await responseStream.WriteAsync(new APIReply { Code = 1, Msg = "上传文件中", Data = apiUpFileModel.FileVersion.ToJson() });
                                            }
                                        }
                                        else if (apiUpFileModel.FileVersion.FileType == EnumFileType.Directory)
                                        {
                                            var filepath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, apiUpFileModel.UpdateLibName, apiUpFileModel.Version.ToString(), apiUpFileModel.FileVersion.FileName);
                                            var filepathInfo = new FileInfo(filepath);
                                            if (!filepathInfo.Directory.Exists)
                                            {
                                                filepathInfo.Directory.Create();
                                            }
                                            lock (lockobj_Libs)
                                            {
                                                var temp_ver = ver.FileVersions.FirstOrDefault(m => m.FileName == apiUpFileModel.FileVersion.FileName);
                                                if (temp_ver == null)
                                                {
                                                    ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                                }
                                                else
                                                {
                                                    ver.FileVersions.Remove(temp_ver);
                                                    ver.FileVersions.Insert(0, apiUpFileModel.FileVersion);
                                                }
                                                HelperServer.SaveLibsData();
                                            }
                                            await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "目录创建成功", Data = apiUpFileModel.FileVersion.ToJson() });
                                        }

                                    }



                                }
                                break;
                            case ApiPaths.Test:
                                {
                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "测试操作" });
                                }
                                break;
                            case ApiPaths.ManageGetlibs:
                                {
                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功", Data = HelperServer.Libs.ToJson() });
                                }
                                break;
                            case ApiPaths.ManageAddlib:
                                {

                                    var req_model = request.Data.JsonTo<ApiVersionModel>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib != null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息已存在, Msg = ErrorMsg.更新库信息已存在.ToString() });
                                        return;
                                    }
                                    lib = new UpdateLib() { Name = req_model.UpdateLibName };
                                    if (lib.Name.IsFileName())
                                    {
                                        var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);
                                        if (!Directory.Exists(System.IO.Path.Combine(libPath, lib.Name)))
                                        {
                                            try
                                            {
                                                Directory.CreateDirectory(System.IO.Path.Combine(libPath, lib.Name));

                                                lock (lockobj_Libs)
                                                {
                                                    HelperServer.Libs.Insert(0, lib);
                                                    HelperServer.SaveLibsData();
                                                }

                                                Helper.Log.Debug("AddLib:" + lib.ToJson());
                                            }
                                            catch (Exception ex)
                                            {
                                                Helper.Log.Error(ex);
                                                await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库已经存在, Msg = ErrorMsg.更新库已经存在.ToString() });
                                            return;
                                        }

                                        await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功", Data = lib.ToJson() });
                                    }
                                    else
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库名格式错误, Msg = ErrorMsg.更新库名格式错误.ToString() });
                                    }
                                }
                                break;
                            case ApiPaths.ManageDellib:
                                {
                                    var req_model = request.Data.JsonTo<ApiVersionModel>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }

                                    var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);

                                    try
                                    {
                                        if (Directory.Exists(System.IO.Path.Combine(libPath, lib.Name)))
                                        {
                                            Directory.Delete(System.IO.Path.Combine(libPath, lib.Name), true);
                                        }
                                        lock (lockobj_Libs)
                                        {
                                            HelperServer.Libs.Remove(lib);
                                            HelperServer.SaveLibsData();
                                        }
                                        Helper.Log.Debug("DelLib:" + lib.ToJson());
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.Log.Error(ex);
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                        return;
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功" });

                                }
                                break;
                            case ApiPaths.ManageAddver:
                                {

                                    var req_model = request.Data.JsonTo<LibVersionInfo>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }
                                    var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);

                                    try
                                    {
                                        lib.LastVersion++;
                                        req_model.Version = lib.LastVersion;
                                        Directory.CreateDirectory(System.IO.Path.Combine(libPath, lib.Name, lib.LastVersion.ToString()));

                                        lock (lockobj_Libs)
                                        {
                                            lib.Configs.Insert(0, req_model);
                                            HelperServer.SaveLibsData();
                                        }

                                        Helper.Log.Debug("AddVer:" + req_model.ToJson());
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.Log.Error(ex);
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                        return;
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功", Data = req_model.ToJson() });

                                }
                                break;
                            case ApiPaths.ManageUpdver:
                                {

                                    var req_model = request.Data.JsonTo<LibVersionInfo>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }
                                    var ver = lib.Configs.FirstOrDefault(m => m.Version == req_model.Version);
                                    if (ver == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.版本库信息不存在, Msg = ErrorMsg.版本库信息不存在.ToString() });
                                        return;
                                    }
                                    var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);

                                    try
                                    {
                                        ver.IsEnablePreReleaseTime = req_model.IsEnablePreReleaseTime;
                                        ver.IsRelease = req_model.IsRelease;
                                        ver.PreReleaseTime = req_model.PreReleaseTime;
                                        ver.ReleaseNotes = req_model.ReleaseNotes;
                                        ver.VersionText = req_model.VersionText;
                                        Directory.CreateDirectory(System.IO.Path.Combine(libPath, lib.Name, lib.LastVersion.ToString()));

                                        lock (lockobj_Libs)
                                        {
                                            HelperServer.SaveLibsData();
                                        }

                                        Helper.Log.Debug("UpdVer:" + req_model.ToJson());
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.Log.Error(ex);
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                        return;
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功" });

                                }
                                break;
                            case ApiPaths.ManageDelver:
                                {
                                    var req_model = request.Data.JsonTo<ApiVersionModel>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }
                                    var ver = lib.Configs.FirstOrDefault(m => m.Version == req_model.Version);
                                    if (ver == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.版本库信息不存在, Msg = ErrorMsg.版本库信息不存在.ToString() });
                                        return;
                                    }
                                    if (ver.IsRelease || (ver.IsEnablePreReleaseTime && DateTime.Now >= ver.PreReleaseTime))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.版本已发布无法删除, Msg = ErrorMsg.版本已发布无法删除.ToString() });
                                        return;
                                    }

                                    var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);

                                    if (Directory.Exists(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString())))
                                    {
                                        try
                                        {
                                            Directory.Delete(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString()), true);
                                            lock (lockobj_Libs)
                                            {
                                                lib.Configs.Remove(ver);
                                                HelperServer.SaveLibsData();
                                            }

                                            Helper.Log.Debug("DelVer:" + ver.ToJson());
                                        }
                                        catch (Exception ex)
                                        {
                                            Helper.Log.Error(ex);
                                            await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            lock (lockobj_Libs)
                                            {
                                                lib.Configs.Remove(ver);
                                                HelperServer.SaveLibsData();
                                            }

                                            Helper.Log.Debug("DelVer:" + ver.ToJson());
                                        }
                                        catch (Exception ex)
                                        {
                                            Helper.Log.Error(ex);
                                            await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                            return;
                                        }
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功", Data = ver.ToJson() });

                                }
                                break;
                            case ApiPaths.ManageDelfile:
                                {
                                    var req_model = request.Data.JsonTo<ApiVersionModel>();

                                    var lib = HelperServer.Libs.FirstOrDefault(m => m.Name == req_model.UpdateLibName);
                                    if (lib == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.更新库信息不存在, Msg = ErrorMsg.更新库信息不存在.ToString() });
                                        return;
                                    }
                                    var ver = lib.Configs.FirstOrDefault(m => m.Version == req_model.Version);
                                    if (ver == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.版本库信息不存在, Msg = ErrorMsg.版本库信息不存在.ToString() });
                                        return;
                                    }
                                    if (ver.IsRelease || (ver.IsEnablePreReleaseTime && DateTime.Now >= ver.PreReleaseTime))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.版本已发布无法删除, Msg = ErrorMsg.版本已发布无法删除.ToString() });
                                        return;
                                    }
                                    var file = ver.FileVersions.FirstOrDefault(m => m.FileName == req_model.FileName);
                                    if (file == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.文件信息不存在, Msg = ErrorMsg.文件信息不存在.ToString() });
                                        return;
                                    }

                                    var libPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath);

                                    if (file.FileType == EnumFileType.File)
                                    {
                                        if (File.Exists(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString(), file.FileName)))
                                        {
                                            try
                                            {
                                                File.Delete(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString(), file.FileName));

                                                lock (lockobj_Libs)
                                                {
                                                    ver.FileVersions.Remove(file);
                                                    HelperServer.SaveLibsData();
                                                }

                                                Helper.Log.Debug("DelFile:" + file.ToJson());
                                            }
                                            catch (Exception ex)
                                            {
                                                Helper.Log.Error(ex);
                                                await responseStream.WriteAsync(new APIReply { Code = -300, Msg = ex.Message });
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            lock (lockobj_Libs)
                                            {
                                                ver.FileVersions.Remove(file);
                                                HelperServer.SaveLibsData();
                                            }
                                            Helper.Log.Debug("DelFileInfo:" + file.ToJson());

                                        }

                                    }
                                    else if (file.FileType == EnumFileType.Directory)
                                    {
                                        if (Directory.Exists(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString(), file.FileName)))
                                        {
                                            try
                                            {
                                                Directory.Delete(System.IO.Path.Combine(libPath, ver.UpdateLibName, ver.Version.ToString(), file.FileName));

                                                lock (lockobj_Libs)
                                                {
                                                    ver.FileVersions.Remove(file);
                                                    HelperServer.SaveLibsData();
                                                }

                                                Helper.Log.Debug("DelFile:" + file.ToJson());
                                            }
                                            catch (Exception ex)
                                            {
                                                Helper.Log.Error(ex);
                                                await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ex.Message });
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            lock (lockobj_Libs)
                                            {
                                                ver.FileVersions.Remove(file);
                                                HelperServer.SaveLibsData();
                                            }
                                            Helper.Log.Debug("DelFileInfo:" + file.ToJson());
                                        }
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "操作成功" });

                                }
                                break;
                            default:
                                {
                                    await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知操作, Msg = ErrorMsg.未知操作.ToString() });
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await responseStream.WriteAsync(new APIReply { Code = (int)ErrorMsg.未知错误, Msg = ErrorMsg.未知错误.ToString() });
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Helper.Log.Error("[----------------- ");
                    Helper.Log.Error(ex);
                    Helper.Log.Error(" -----------------]");
                }
            }

        }

    }
}
