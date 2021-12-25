using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 直接执行命令更新,不使用更新库配置
    /// </summary>
    [Verb("run", HelpText = "直接执行命令更新,不使用更新库配置")]
    public class RunOptions: BaseOptions
    {
    }
}
