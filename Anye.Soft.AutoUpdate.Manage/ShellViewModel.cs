using Anye.Soft.AutoUpdate.Manage.Models;
using Anye.Soft.AutoUpdate.Manage.Views;
using Anye.Soft.Common;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage
{
    public class ShellViewModel : Conductor<IScreen>
    {
        public string Title { get; set; }
        public ManageDataModel SelDataModel { get; set; }
        public ObservableCollection<ManageDataModel> DataModels { get; set; } = MHelper.ManageDataConfig;
        private IWindowManager windowManager;
        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            var p = System.Reflection.Assembly.GetExecutingAssembly().CustomAttributes.FirstOrDefault(m => m.AttributeType == typeof(System.Reflection.AssemblyProductAttribute));
            Title = p.ConstructorArguments.FirstOrDefault().Value?.ToString();
        }


        public void AddService()
        {
            var data = new ManageDataModel() { Name = "", IPort = "", Remarks = "", ServerKey = "" };
            var viewModel = new ServiceManageViewModel(data, false);
            bool? result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                if (!viewModel.ErrorInfo.IsNullOrEmptyOrSpace())
                {
                    this.windowManager.ShowDialog(new Views.MessageBoxViewModel(viewModel.ErrorInfo));
                }
                MHelper.LoadManageDataConfig();
            }
        }

        public void EditService()
        {
            if (SelDataModel == null)
            {
                this.windowManager.ShowMessageBox("请选择服务");
                return;
            }
            var viewModel = new ServiceManageViewModel(SelDataModel, true);
            bool? result = this.windowManager.ShowDialog(viewModel);
            if (result.GetValueOrDefault(true))
            {
                if (!viewModel.ErrorInfo.IsNullOrEmptyOrSpace())
                {
                    this.windowManager.ShowDialog(new Views.MessageBoxViewModel(viewModel.ErrorInfo));
                }
                MHelper.LoadManageDataConfig();
            }
        }

        public void DelService()
        {
            if (SelDataModel == null)
            {
                this.windowManager.ShowDialog(new Views.MessageBoxViewModel("请选择服务"));
                return;
            }
            var rz = this.windowManager.ShowDialog(new Views.MessageBoxViewModel("确定要删除:[" + SelDataModel.Name + "]?", true));
            if (rz == true)
            {
                MHelper.DelManageData(SelDataModel);
                MHelper.LoadManageDataConfig();
            }

        }

        public ContentControlViewModel ControlViewModel { get; set; }

        public void MouseDoubleClick(ManageDataModel manageData)
        {
            ControlViewModel = new ContentControlViewModel(windowManager, manageData);
            this.ActivateItem(ControlViewModel);
        }
    }
}
