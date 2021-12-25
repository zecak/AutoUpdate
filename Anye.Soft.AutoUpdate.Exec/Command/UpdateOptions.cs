using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 执行更新
    /// </summary>
    [Verb("update", HelpText = "执行更新")]
    public class UpdateOptions
    {
        /// <summary>
        /// 更新库配置名称
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "更新库配置名称")]
        public string Name { get; set; }
    }
}
