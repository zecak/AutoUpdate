using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 创建快捷方式
    /// </summary>
    [Verb("lnk", HelpText = "创建快捷方式")]
    public class LnkOptions
    {

        /// <summary>
        /// 快捷方式名称
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "快捷方式名称(linux专用目标路径)")]
        public string Name { get; set; }

        /// <summary>
        /// 源路径
        /// </summary>
        [Option('s', "source", Required = true, HelpText = "源路径(linux专用源路径)")]
        public string Source { get; set; }

        /// <summary>
        /// 快捷方式路径
        /// </summary>
        [Option('p', "path", Required = false, HelpText = "快捷方式路径(默认使用源路径)(特殊路径:[桌面],[Desktop])")]
        public string LnkPath { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        [Option('i', "icon", Required = false, HelpText = "图标路径")]
        public string Icon { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Option('r', "remark", Required = false, HelpText = "备注")]
        public string Remark { get; set; }

    }
}
