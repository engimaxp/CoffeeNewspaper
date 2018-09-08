using System.Threading.Tasks;
using CN_Presentation.ViewModel.Base;

namespace CN_Presentation.ViewModel.Form
{
    public abstract class FormBaseViewModel : BaseViewModel
    {
        /// <summary>
        ///     The form click confirm will call this methods
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> Confirm();

        /// <summary>
        ///     if the child model is running confirm method ,this value is true
        /// </summary>
        public bool ConfirmIsRunning {get;set;}
    }
}