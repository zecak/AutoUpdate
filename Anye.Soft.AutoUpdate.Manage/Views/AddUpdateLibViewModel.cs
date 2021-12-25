using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.Common;
using Anye.Soft.Common.Models;
using AnyeSoft.Common.Service;
using Grpc.Core;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    public class AddUpdateLibViewModel : Screen
    {
        public gRPC.gRPCClient RPCClient { get; set; }
        public ManageDataModel DataModel { get; set; }

        public string UpdateLibName { get; set; }
        public string ErrorInfo { get; set; }

        public AddUpdateLibViewModel(gRPC.gRPCClient rpcClient, ManageDataModel dataModel)
        {
            RPCClient = rpcClient;
            DataModel = dataModel;
        }

        public async void Add()
        {
            if (!UpdateLibName.IsFileName())
            {
                ErrorInfo = "名称格式错误";
                this.RequestClose(true);
                return;
            }

            await AddLib();

        }

        private async Task AddLib()
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
                var info = new APIRequest() { ApiPath = ApiPaths.ManageAddlib, AppID = DataModel.AdminName, Data = new ApiVersionModel() { UpdateLibName= UpdateLibName }.ToJson(), Time = DateTime.Now.ToTimestamp() };
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
