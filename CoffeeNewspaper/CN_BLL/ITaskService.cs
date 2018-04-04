using CN_Model;

namespace CN_BLL
{
    public interface ITaskService
    {
        int CreateATask(CNTask task);

        bool DeleteTask(int taskId, bool force = false);

        bool RemoveATask(int taskId, bool force = false);

        bool RecoverATask(int taskId);

        bool StartATask(int taskId);

        bool PauseATask(int taskId);

        bool FinishATask(int taskId);

        bool FailATask(int taskId,string reason);
    }
}
