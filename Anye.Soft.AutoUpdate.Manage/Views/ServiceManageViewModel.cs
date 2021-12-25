using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.Common;
using Anye.Soft.Common.Security;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    public class ServiceManageViewModel : Screen
    {
        public bool IsEdit { get; set; }
        public ManageDataModel DataModel { get; set; }
        public string ErrorInfo { get; set; }
        public ServiceManageViewModel(ManageDataModel manageData, bool isEdit = false)
        {
            DataModel = manageData;
            IsEdit = isEdit;
        }

        public void Add()
        {
            if (DataModel.Name.IsNullOrEmptyOrSpace())
            {
                ErrorInfo = "名称必填";
                this.RequestClose(true);
                return;
            }
            var strlist = DataModel.IPort.Split(":");
            if (strlist.Length!=2)
            {
                ErrorInfo = "IP(或域名)和端口格式错误,正确如:127.0.0.1:9999或update.xxxxx.com:9999";
                this.RequestClose(true);
                return;
            }
            ErrorInfo = MHelper.AddManageData(DataModel);
            this.RequestClose(true);
        }

        public void Edit()
        {
            if (DataModel.Name.IsNullOrEmptyOrSpace())
            {
                ErrorInfo = "名称必填";
                this.RequestClose(true);
                return;
            }
            var strlist = DataModel.IPort.Split(":");
            if (strlist.Length != 2)
            {
                ErrorInfo = "IP(或域名)和端口格式错误,正确如:127.0.0.1:9999或update.xxxxx.com:9999";
                this.RequestClose(true);
                return;
            }
            ErrorInfo = MHelper.EditManageData(DataModel);
            this.RequestClose(true);
        }

        public void Close()
        {
            this.RequestClose(false);
        }
    }
}
