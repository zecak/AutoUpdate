using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public class ApiUpFileModel
    {
        public string UpdateLibName { get; set; }
        public int Version { get; set; }
        public FileVersionInfo FileVersion { get; set; }
    }
}
