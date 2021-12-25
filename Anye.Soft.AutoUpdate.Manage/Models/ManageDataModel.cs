using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Models
{
    public class ManageDataModel
    {

        public Guid GID { get; set; } = Guid.Empty;
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP(或域名)和端口,如:127.0.0.1:9999或update.xxxxx.com:9999
        /// </summary>
        public string IPort { get; set; }

        /// <summary>
        /// 管理账号
        /// </summary>
        public string AdminName { get; set; }
        /// <summary>
        /// 管理密钥
        /// </summary>
        public string ServerKey { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public ObservableCollection<UpdateLibUI> Libs { get; set; } = new ObservableCollection<UpdateLibUI>();
    }
}
