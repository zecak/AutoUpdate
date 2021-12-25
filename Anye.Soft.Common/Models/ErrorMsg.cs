using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.Common.Models
{
    public enum ErrorMsg
    {
        未知错误 = 111,
        未知操作 = 999,
        电子签名不一致 = 1000,
        无权限 = 1001,
        更新库信息已存在 = 2000,
        更新库已经存在 = 2001,
        更新库名格式错误 = 2002,
        更新库信息不存在 = 2003,
        更新库不存在 = 2004,
        版本库信息不存在 = 3000,
        版本已发布无法删除 = 3001,
        文件信息不存在 = 3002,
        版本库不存在 = 3003,
        请求参数必填 = 4000,
        请求参数更新库名称必填 = 4001,

    }
}
