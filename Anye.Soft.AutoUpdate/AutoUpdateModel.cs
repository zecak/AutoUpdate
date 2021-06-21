using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate
{
    public class AutoUpdateModel
    {
        public bool IsUseAutoUpdateJson { get; set; } 
        public string ClientName { get; set; }
        public int ClientVersion { get; set; }
        public string UpdatePath { get; set; }
        public string FileName { get; set; }

        public EnumAutoUpdate EnumAutoUpdate { get; set; }
        public bool IsConsoleReadKey { get; set; }
    }

}
