using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 默认命令
    /// </summary>
    [Verb("default", true, HelpText = "默认命令")]
    public class DefaultOptions
    {
        /// <summary>
        /// 显示版本信息
        /// </summary>
        [Option('v', "ver", HelpText = "显示版本信息")]
        public bool ShowVersion { get; set; }

    }

}
