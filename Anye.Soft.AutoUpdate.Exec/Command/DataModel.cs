using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    public class DataModel
    {
        public string Target { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string UpdateLibName { get; set; }

        public int CurrentVersion { get; set; }

        public string Path { get; set; }
        public bool IsService { get; set; }
        public string FileName { get; set; }
    }
}
