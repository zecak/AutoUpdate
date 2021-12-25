using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public class FileVersionInfo
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string MD5 { get; set; }
        public EnumFileType FileType { get; set; } = EnumFileType.File;
        public EnumUpdateAction Action { get; set; } = EnumUpdateAction.None;

    }
}
