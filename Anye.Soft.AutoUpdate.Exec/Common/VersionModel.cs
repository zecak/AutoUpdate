using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Common
{
    public class VersionModel
    {
        public string UpdateLibName { get; set; }
        public int CurrentVersion { get; set; }
        public string VersionText { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime UpdateDateTime { get; set; } 
    }
}
