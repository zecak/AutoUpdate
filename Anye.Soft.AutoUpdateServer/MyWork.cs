using Anye.Soft.Common;
using AnyeSoft.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdateServer
{
    public class MyWork
    {
        Grpc.Core.Server server = null;

        public void Start()
        {
            try
            {
                Helper.Log.Info(Helper.ServerSetting.ServerIP + ":" + Helper.ServerSetting.ServerPort);
                Helper.Log.Info("[暗夜软件自动更新服务]");
                Helper.Log.Debug("--- gRPC:监听端口[" + Helper.ServerSetting.ServerPort + "] ---");
                server = new Grpc.Core.Server
                {
                    Services = { gRPC.BindService(new GrpcImpl()) },
                    Ports = { new Grpc.Core.ServerPort(Helper.ServerSetting.ServerIP, Helper.ServerSetting.ServerPort.ToInt(), Grpc.Core.ServerCredentials.Insecure) }
                };

                server.Start();
                Helper.Log.Info("[gRPC]服务->启动成功");

                Helper.Log.Debug("监听目录[" + System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Helper.SettingAutoUpdateSetting.UpdatePath) + "]");
                HelperServer.MonitorDirectory();

            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
            }

        }

        public void Stop()
        {
            try
            {
                server?.ShutdownAsync().Wait();
                server = null;

            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
            }

        }
    }
}
