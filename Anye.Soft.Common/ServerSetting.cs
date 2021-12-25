using Anye.Soft.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common
{
    public class ServerSetting
    {
        public string ServerIP { get; set; }
        public string ServerPort { get; set; }

        public UserModel Admin { get; set; }

        public UserModel Updater { get; set; }

    }
}
