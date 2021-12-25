using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using Grpc.Core;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    public class AddVersionLibViewModel : Screen
    {
        public CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentCulture;

        public gRPC.gRPCClient RPCClient { get; set; }
        public ManageDataModel DataModel { get; set; }
        public UpdateLibUI SelectUpdateLib { get; set; }

        public LibVersionInfoUI LibVersionInfoUI { get; set; }

        public string ErrorInfo { get; set; }
        public bool IsEdit { get; set; }

        public string Title { get; set; }

        public AddVersionLibViewModel(gRPC.gRPCClient rpcClient, ManageDataModel dataModel, UpdateLibUI selectUpdateLib, bool isEdit = false)
        {
            RPCClient = rpcClient;
            DataModel = dataModel;
            SelectUpdateLib = selectUpdateLib;
            IsEdit = isEdit;

            LibVersionInfoUI = IsEdit ? SelectUpdateLib.LibVersionInfoUI : new LibVersionInfoUI() { UpdateLibName = SelectUpdateLib.Name };


            Title = IsEdit ? "版本" + LibVersionInfoUI.Version : "新增版本库";

        }

        public async void Add()
        {
            if (SelectUpdateLib.Configs.Any(m => !m.IsRelease))
            {
                ErrorInfo = "已有未发布版本,无法再添加";
                this.RequestClose(true);
                return;
            }
            await AddVer();
        }

        private async Task AddVer()
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
                            var ver = note.Data.JsonTo<LibVersionInfoUI>();
                            if (ver != null)
                            {
                                SelectUpdateLib.Configs.Insert(0, ver);
                            }
                            ErrorInfo = "";
                            this.RequestClose(true);
                        });

                    }
                    else
                    {
                        ErrorInfo = note.Msg;
                    }


                }

            });

            try
            {
                var info = new APIRequest() { ApiPath = ApiPaths.ManageAddver, AppID = DataModel.AdminName, Data = LibVersionInfoUI.ToJson(), Time = DateTime.Now.ToTimestamp(), };
                info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                ErrorInfo = "连接失败";
            }

            await responseReaderTask;
        }

        public async void Save()
        {
            await SaveVer();
        }

        private async Task SaveVer()
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
                            ErrorInfo = "";
                            this.RequestClose(true);
                        });

                    }
                    else
                    {
                        ErrorInfo = note.Msg;
                    }


                }

            });

            try
            {
                var info = new APIRequest() { ApiPath = ApiPaths.ManageUpdver, AppID = DataModel.AdminName, Data = LibVersionInfoUI.ToJson(), Time = DateTime.Now.ToTimestamp(), };
                info.Sign = (info.AppID + info.Data + info.Time + DataModel.ServerKey).ToMd5();

                await chatCall.RequestStream.WriteAsync(info);
                await chatCall.RequestStream.CompleteAsync();
            }
            catch (RpcException ex)
            {
                ErrorInfo = "连接失败";
            }

            await responseReaderTask;
        }

        public void Close()
        {
            this.RequestClose(false);
        }
    }
}
