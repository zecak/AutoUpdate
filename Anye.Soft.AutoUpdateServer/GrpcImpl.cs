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

namespace Anye.Soft.AutoUpdateServer
{
    public class GrpcImpl : gRPC.gRPCBase
    {

        public bool CheckSign(APIRequest request)
        {
            var serverKey = Helper.ServerSetting.ServerKey;
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
                return Task.FromResult(new APIReply { Code = -100, Msg = "未知操作" });
            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
                return Task.FromResult(new APIReply { Code = -1, Msg = "未知错误" });
            }
        }

        public override async Task Chat(IAsyncStreamReader<APIRequest> requestStream, IServerStreamWriter<APIReply> responseStream, ServerCallContext context)
        {

            while (await requestStream.MoveNext())
            {
                try
                {
                    var request = requestStream.Current;

                    if (!CheckSign(request))
                    {
                        APIReply resp = new APIReply() { };
                        resp.Code = -200;
                        resp.Msg = "电子签名不一致";
                        await responseStream.WriteAsync(resp);
                        return;
                    }

                    try
                    {
                        switch (request.ApiPath)
                        {
                            case "/get/version":
                                {
                                    var vmodel = request.Data.JsonTo<ApiVersionModel>();
                                    if (vmodel == null)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -104, Msg = "请求参数必填" });
                                        return;
                                    }
                                    if (vmodel.ClientName.IsNullOrEmptyOrSpace())
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -103, Msg = "请求参数客户端名称必填" });
                                        return;
                                    }
                                    var clientUpdInfoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, vmodel.ClientName);
                                    if (!Directory.Exists(clientUpdInfoPath))
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -101, Msg = "无客户端更新信息" });
                                        return;
                                    }
                                    var vers = Directory.GetDirectories(clientUpdInfoPath);
                                    if (vers.Length == 0)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = -102, Msg = "无客户端版本信息" });
                                        return;
                                    }
                                    var versions = vers.Reverse().ToArray().ToList();
                                    versions.RemoveAll(m => m.ToInt() > vmodel.ClientVersion);//排除当前版本和以前版本(根据目录名排除)
                                    if (versions.Count == 0)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = 1, Msg = "无版本更新无文件" });
                                        return;
                                    }
                                    var needUpdates = HelperServer.UpdateConfigs.FindAll(m => m.ClientName == vmodel.ClientName && m.ClientVersion > vmodel.ClientVersion);//再排除实际版本配置文件
                                    if (needUpdates.Count == 0)
                                    {
                                        await responseStream.WriteAsync(new APIReply { Code = 1, Msg = "已是最新版本" });
                                        return;
                                    }
                                    foreach (var update in needUpdates)
                                    {
                                        var tlist = update.FileVersions.ToList().FindAll(m => m.Action != EnumUpdateAction.无);
                                        update.FileVersions.Clear();
                                        foreach (var t in tlist)
                                        {
                                            update.FileVersions.Add(t);
                                        }
                                    }

                                    await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "有版本更新", Data = needUpdates.ToJson() });
                                }
                                break;
                            case "/file/down":
                                {
                                    var downFileModel = request.Data.JsonTo<ApiDownFileModel>();
                                    var fullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath, downFileModel.ClientName, downFileModel.ClientVersion.ToString(), downFileModel.FileName);
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
                                            await responseStream.WriteAsync(new APIReply { Code = 0, Msg = "下载文件", FileBlock = currentTimes, FileContents = sbytes });
                                            currentTimes++;
                                        }
                                    }
                                }
                                break;
                            case "/file/up":
                                {

                                }
                                break;
                            default:
                                {
                                    await responseStream.WriteAsync(new APIReply { Code = -100, Msg = "未知操作" });
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await responseStream.WriteAsync(new APIReply { Code = -1, Msg = "未知错误" });
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
