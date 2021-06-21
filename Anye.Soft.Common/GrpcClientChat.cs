using AnyeSoft.Common.Service;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anye.Soft.Common
{
    public class GrpcFailedEventArgs : EventArgs
    {
        public RpcException Exception { get; set; }
    }
    public class GrpcClientChat
    {
        public delegate void GrpcFailedHandler(object sender, GrpcFailedEventArgs args);

        private event GrpcFailedHandler _newchatFailed;

        public event GrpcFailedHandler NewChatFailed
        {
            add => _newchatFailed += value;
            remove => _newchatFailed -= value;
        }
        public event Action<GrpcClientChat, APIReply> NewChating;
        public event Action<GrpcClientChat> NewChated;
        public string Target { get; set; }
        Channel channel = null;
        gRPC.gRPCClient client = null;
        AsyncDuplexStreamingCall<APIRequest, APIReply> NewChatCall = null;

        public GrpcClientChat(string target)
        {
            Target = target;

            channel = new Channel(Target, ChannelCredentials.Insecure);
            client = new gRPC.gRPCClient(channel);

            NewChat();
        }

        void NewChat()
        {

            NewChatCall = client.Chat();

            //taskTokenNewChatCall = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await NewChatCall.ResponseStream.MoveNext())
                    {
                        var note = NewChatCall.ResponseStream.Current;
                        NewChating?.Invoke(this, note);
                    }
                    NewChated?.Invoke(this);
                });

                try
                {
                    //await call_temp.RequestStream.CompleteAsync();
                }
                catch (RpcException ex)
                {
                    _newchatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
                }

                await responseReaderTask;
            });
        }

        public async Task SendInfo(APIRequest reqData)
        {
            try
            {
                if(reqData==null)
                {
                    return;
                }
                await NewChatCall.RequestStream.WriteAsync(reqData);
            }
            catch (RpcException ex)
            {
                _newchatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });

                NewChat();
            }

        }

        public async Task CompleteChat()
        {
            try
            { 
               await NewChatCall.RequestStream.CompleteAsync(); 
            }
            catch (RpcException ex)
            {
                _newchatFailed?.Invoke(this, new GrpcFailedEventArgs() { Exception = ex });
            }

        }
    }
}
