using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 删除更新库配置
    /// </summary>
    [Verb("del", HelpText = "删除更新库配置")]
    public class DelOptions
    {
        /// <summary>
        /// 更新库配置名称
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "更新库配置名称")]
        public string Name { get; set; }

    }
}
