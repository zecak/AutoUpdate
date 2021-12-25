using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public class LibVersionInfo
    {
        /// <summary>
        /// 更新库名
        /// </summary>
        public string UpdateLibName { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 版本描述
        /// </summary>
        public string VersionText { get; set; }
        /// <summary>
        /// 是否发布,true则可更新,false则不可更新
        /// </summary>
        public bool IsRelease { get; set; }
        /// <summary>
        /// 发行说明
        /// </summary>
        public string ReleaseNotes { get; set; }
        /// <summary>
        /// 是否启用预发布时间
        /// </summary>
        public bool IsEnablePreReleaseTime { get; set; }
        /// <summary>
        /// 预发布时间,当IsEnablePreReleaseTime为true时生效
        /// </summary>
        public DateTime PreReleaseTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 文件信息
        /// </summary>
        public List<FileVersionInfo> FileVersions { get; set; } = new List<FileVersionInfo>();
    }
}
