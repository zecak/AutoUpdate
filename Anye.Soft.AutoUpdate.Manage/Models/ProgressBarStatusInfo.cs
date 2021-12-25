using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anye.Soft.AutoUpdate.Manage.Models
{
    public class ProgressBarStatusInfo
    {
        public string ActionName { get; set; } = "";
        public string ActionInfo { get; set; } = "";
        public int ActionCount { get; set; } = 100;
        public int ActionCurIndex { get; set; } = 0;

        public int BlockActionCount { get; set; } = 10;
        public int BlockActionCurIndex { get; set; } = 0;

        public bool IsCompleted { get; set; } = true;

    }
}
