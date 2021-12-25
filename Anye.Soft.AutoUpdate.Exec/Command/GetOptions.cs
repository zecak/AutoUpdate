using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 查看更新库配置
    /// </summary>
    [Verb("get", HelpText = "查看更新库配置")]
    public class GetOptions
    {
        /// <summary>
        /// 更新库配置名称
        /// </summary>
        [Option('n', "name", Required = false, HelpText = "更新库配置名称")]
        public string Name { get; set; }

        /// <summary>
        /// 是否格式化Json字符串
        /// </summary>
        [Option('f', "format", Required = false, HelpText = "是否格式化Json字符串")]
        public bool IsFormatting { get; set; }

    }
}
