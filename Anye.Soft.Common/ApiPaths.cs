using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common
{
    public class ApiPaths
    {
        /// <summary>
        /// 获取所有更新版本
        /// </summary>
        public const string GetAllVersion = "/getall/version";
        /// <summary>
        /// 获取指定更新版本
        /// </summary>
        public const string GetVersion = "/get/version";
        /// <summary>
        /// 下载文件
        /// </summary>
        public const string FileDown = "/file/down";
        /// <summary>
        /// 上传文件
        /// </summary>
        public const string FileUp = "/file/up";
        /// <summary>
        /// 测试
        /// </summary>
        public const string Test = "/test";
        /// <summary>
        /// 获取更新库
        /// </summary>
        public const string ManageGetlibs = "/manage/getlibs";
        /// <summary>
        /// 添加更新库
        /// </summary>
        public const string ManageAddlib = "/manage/addlib";
        /// <summary>
        /// 删除更新库
        /// </summary>
        public const string ManageDellib = "/manage/dellib";
        /// <summary>
        /// 添加版本库
        /// </summary>
        public const string ManageAddver = "/manage/addver";
        /// <summary>
        /// 设置版本库
        /// </summary>
        public const string ManageUpdver = "/manage/updver";
        /// <summary>
        /// 删除版本库
        /// </summary>
        public const string ManageDelver = "/manage/delver";
        /// <summary>
        /// 删除文件
        /// </summary>
        public const string ManageDelfile = "/manage/delfile";

    }
}
