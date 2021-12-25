using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public class ApiVersionModel
    {
        /// <summary>
        /// 更新库名称
        /// </summary>
        public string UpdateLibName { get; set; }
        /// <summary>
        /// 版本库
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 用于删除文件
        /// </summary>
        public string FileName { get; set; }
    }
}
