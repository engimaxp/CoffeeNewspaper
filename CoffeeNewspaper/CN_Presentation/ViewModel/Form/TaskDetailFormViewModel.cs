using CN_Core;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Form
{
    public class TaskDetailFormViewModel:BaseViewModel
    {
        public TaskCurrentStatus Status { get; set; }

        public bool IsFail { get; set; }

        public bool IsPending  => Status == TaskCurrentStatus.PENDING;

        public string FailReason { get; set; }

        public string PendingReason { get; set; }
    }
}