using Anye.Soft.Common;
using AnyeSoft.Common.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Server
{
    public class Worker : BackgroundService
    {
        Grpc.Core.Server server = null;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var p = System.Reflection.Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(m => m.AttributeType == typeof(System.Reflection.AssemblyProductAttribute));
                var appInfo = p.ConstructorArguments.FirstOrDefault().Value?.ToString();

                await Task.Factory.StartNew(() =>
                {
                    Helper.Log.Info(appInfo);
                    Helper.Log.Info(Helper.ServerSetting.ServerIP + ":" + Helper.ServerSetting.ServerPort);
                    
                    server = new Grpc.Core.Server
                    {
                        Services = { gRPC.BindService(new GrpcImpl()) },
                        Ports = { new Grpc.Core.ServerPort(Helper.ServerSetting.ServerIP, Helper.ServerSetting.ServerPort.ToInt(), Grpc.Core.ServerCredentials.Insecure) }
                    };

                    server.Start();
                    Helper.Log.Info("更新服务 --> 启动成功!");

                }, stoppingToken);

            }
            catch (Exception ex)
            {
                Helper.Log.Error(ex);
            }

        }

    }
}
