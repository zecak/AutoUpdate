using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Views
{
    public class MessageBoxViewModel : Screen
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public bool ShowCancelButton { get; set; }

        public MessageBoxViewModel(string text, bool showCancelButton = false)
        {
            Title = "温馨提示";
            Text = text;
            ShowCancelButton = showCancelButton;
        }
        public MessageBoxViewModel(string title, string text,bool showCancelButton=false)
        {
            Title = title;
            Text = text;
            ShowCancelButton = showCancelButton;
        }

        public void OKButton()
        {
            this.RequestClose(true);
        }

        public void CancelButton()
        {
            this.RequestClose(false);
        }
    }
}
