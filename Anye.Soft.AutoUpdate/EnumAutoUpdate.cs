using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate
{
    /// <summary>
    /// 自动升级类型
    /// </summary>
    public enum EnumAutoUpdate
    {
        /// <summary>
        /// 普通应用程序类型
        /// </summary>
        Client,
        /// <summary>
        /// 系统服务类型
        /// </summary>
        Service,
        /// <summary>
        /// web站点类型
        /// </summary>
        Web,
    }
}
