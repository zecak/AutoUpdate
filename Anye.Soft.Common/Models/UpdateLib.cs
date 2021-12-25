using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public class UpdateLib
    {
        /// <summary>
        /// 更新库名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 最后版本,用于创建版本库自动累加
        /// </summary>
        public int LastVersion { get; set; }

        public List<LibVersionInfo> Configs { get; set; } = new List<LibVersionInfo>();

        public int GetLastReleaseVersion()
        {
            var config = Configs.FirstOrDefault(m => m.IsRelease || (m.IsEnablePreReleaseTime && DateTime.Now >= m.PreReleaseTime));
            if (config != null)
            {
                return config.Version;
            }
            return 0;
        }
        public List<LibVersionInfo> GetReleaseVersions(int curVer)
        {
            return Configs.FindAll(m => m.Version > curVer && (m.IsRelease || (m.IsEnablePreReleaseTime && DateTime.Now >= m.PreReleaseTime))).OrderBy(m => m.Version).ToList();

        }

        public LibVersionInfo GetReleaseVersion(int curVer)
        {
            return Configs.FirstOrDefault(m => m.Version == curVer);
        }
    }
}
