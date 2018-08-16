using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces;
using CN_Core.Interfaces.Service;
using CN_Presentation.ViewModel.Controls;
using CN_Presentation.ViewModel.Dialog;

namespace CN_Presentation.Utilities
{
    public static class TaskOperatorHelper
    {
        public static async Task WrapException(Func<Task> doWork)
        {
            try
            {
                await doWork();
            }
            catch (Exception exception)
            {
                await IoC.Get<IUIManager>()
                    .ShowMessage(new MessageBoxDialogViewModel
                    {
                        Title = "Error！",
                        Message = exception.Message
                    });
            }
        }

        public static Func<Task<bool>> DeleteTask(bool force, CNTask targetTask)
        {
            return async () =>
            {
                var result = true;
                try
                {
                    result = await IoC.Get<ITaskService>().DeleteTask(targetTask.TaskId, force);
                    //refresh task
                    await IoC.Get<TaskListViewModel>().RefreshSpecificTaskItem(targetTask.TaskId);
                }
                catch (TaskHasChildTasksException)
                {
                    await IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteTask(true, targetTask))
                    {
                        CofirmText = "Confirm",
                        CancelText = "Cancel",
                        Message = "This task has child tasks",
                        SecondaryMessage = "Do you really want delete it along with its child tasks?",
                    });
                }
                catch (TaskHasSufTasksException)
                {
                    await IoC.Get<IUIManager>().ShowConfirm(new ConfirmDialogBoxViewModel(DeleteTask(true, targetTask))
                    {
                        CofirmText = "Confirm",
                        CancelText = "Cancel",
                        Message = "This task has suf tasks,",
                        SecondaryMessage = "Do you really want delete it along with its suf tasks?",
                    });
                }
                catch (Exception exception)
                {
                    await IoC.Get<IUIManager>()
                        .ShowMessage(new MessageBoxDialogViewModel
                        {
                            Title = "Error！",
                            Message = exception.Message
                        });
                }
                return result;
            };
        }

    }
}