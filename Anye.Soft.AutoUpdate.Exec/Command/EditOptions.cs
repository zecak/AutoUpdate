using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Exec.Command
{
    /// <summary>
    /// 编辑更新库配置
    /// </summary>
    [Verb("edit", HelpText = "编辑更新库配置")]
    public class EditOptions 
    {
        /// <summary>
        /// 更新库配置名称
        /// </summary>
        [Option('n', "name", Required = true, HelpText = "更新库配置名称")]
        public string Name { get; set; }

        /// <summary>
        /// 远程地址(ip:port)
        /// </summary>

        [Option('t', "target", Required = false, HelpText = "远程地址(ip:port)")]
        public string Target { get; set; } = null;

        /// <summary>
        /// 授权账号
        /// </summary>

        [Option('u', "user", Required = false, HelpText = "授权账号")]
        public string User { get; set; } = null;

        /// <summary>
        /// 授权密码
        /// </summary>

        [Option('k', "key", Required = false, HelpText = "授权密码")]
        public string Key { get; set; } = null;

        /// <summary>
        /// 指定更新库名称
        /// </summary>

        [Option('l', "library", Required = false, HelpText = "指定更新库名称")]
        public string UpdateLibName { get; set; } = null;

        /// <summary>
        /// 更新库当前版本
        /// </summary>

        [Option('c', "curver", Required = false, HelpText = "更新库当前版本(用于存储本地版本)")]
        public int? CurrentVersion { get; set; } = null;

        /// <summary>
        /// 要更新程序的路径(未指定则使用更新库名称,作为更新程序目录)
        /// </summary>

        [Option('p', "path", Required = false, HelpText = "要更新程序的路径(未指定则需要手动使用更新库名称,作为更新程序目录)")]
        public string Path { get; set; } = null;

        /// <summary>
        /// 指定更新程序类型属于服务
        /// </summary>

        [Option('s', "service", Required = false, HelpText = "指定更新程序类型属于服务")]
        public bool? IsService { get; set; } = null;

        /// <summary>
        /// 依靠什么启动程序的主程序名称,用于linux;
        /// </summary>
        [Option('e', "execname", Required = false, HelpText = "在linux下用于启动程序的主程序,如:bash或dotnet")]
        public string ExecName { get; set; }

        /// <summary>
        /// 要更新程序的名称(用于关闭/启动程序或服务),windows下如:Anye.Soft.AutoUpdate.Exec.exe或服务名称,linux下如:Anye.Soft.AutoUpdate.Exec.dll或服务名称
        /// </summary>
        [Option('f', "filename", Required = false, HelpText = "要更新程序的名称(用于关闭/启动程序或服务),windows下如:Anye.Soft.AutoUpdate.Exec.exe或服务名称,linux下如:Anye.Soft.AutoUpdate.Exec.dll或服务名称")]
        public string FileName { get; set; } = null;
    }
}
