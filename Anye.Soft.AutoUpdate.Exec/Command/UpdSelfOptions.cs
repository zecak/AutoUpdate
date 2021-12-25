using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 更新自己
    /// </summary>
    [Verb("updself", HelpText = "更新自己")]
    public class UpdSelfOptions: BaseOptions
    {

        [Option('b', "base", Required = true, HelpText = "需要更新的路径")]
        public string BasePath { get; set; }

        [Option('d', "down", Required = true, HelpText = "下载的程序目录")]
        public string DownUpateLibPath { get; set; }

        [Option('m', "mf", Required = true, HelpText = "备份目标路径")]
        public string DestPath { get; set; } 

    }
}
