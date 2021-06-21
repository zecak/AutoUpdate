using Anye.Soft.Common;
using System;
using Topshelf;

namespace Anye.Soft.AutoUpdateServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<MyWork>(t =>
                    {
                        t.ConstructUsing(n => new MyWork());
                        t.WhenStarted(tc => tc.Start());
                        t.WhenStopped(tc => tc.Stop());
                    });

                    x.RunAsLocalSystem();
                    x.StartAutomatically();
                    x.SetDescription(Helper.ServerSetting.Description);
                    x.SetDisplayName(Helper.ServerSetting.DisplayName);
                    x.SetServiceName(Helper.ServerSetting.ServiceName);
                });
            }
            catch (Exception ex)
            {
                Helper.Log.Error("[--------------");
                Helper.Log.Error(ex.Message);
                Helper.Log.Error(ex);
                Helper.Log.Error(" --------------]");
            }
        }
    }
}
