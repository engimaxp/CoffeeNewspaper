using System;
using System.Collections.Generic;
using System.Linq;

namespace CN_Model
{
    public class CNRoot
    {
        public CNRoot()
        {
            TaskList = new List<CNTask>();
            _memoList = new List<CNMemo>();
        }

        public List<CNTask> TaskList { get; }

        private List<CNMemo> _memoList;

        public List<CNMemo> MemoList
        {
            get
            {
                if (_memoList != null && _memoList.Count > 0)
                {
                    var distcount = _memoList.Select(r => r.MemoId).Distinct().ToList();
                    if (distcount.Count() != _memoList.Count)
                    {
                        List<CNMemo> distinctMemos = new List<CNMemo>();
                        distcount.ForEach(x => distinctMemos.Add(_memoList.FirstOrDefault(y => y.MemoId == x)));
                        _memoList = distinctMemos;
                    }
                }
                return _memoList;
            }
            set { _memoList = value; }
        }

        public CNTask GetFirstTask()
        {
            return TaskList.FirstOrDefault();
        }
        public CNTask GetTaskById(int taskid)
        {
            return TaskList.FirstOrDefault(r=>r.TaskId == taskid)??new CNTask();
        }

        /// <summary>
        /// if no memo is found return new memo
        /// </summary>
        /// <param name="memoid"></param>
        /// <returns></returns>
        public CNMemo GetMemoById(string memoid)
        {
            var globalMemo = MemoList.FirstOrDefault(r => r.MemoId.Equals(memoid));
            if (globalMemo != null)
            {
                return globalMemo;
            }
            return (TaskList.Select(x => x.GetAllMemos()).Where(r => r != null && r.Count > 0).ToList().FirstOrDefault(x => x.FirstOrDefault(r => r.MemoId.Equals(memoid) ) != null)??new List<CNMemo>()).FirstOrDefault(t => t.MemoId == memoid);
        }

        public void AddOrUpdateGlobalMemo(CNMemo newMemo)
        {
            if (MemoList.Exists(r => r.MemoId == newMemo.MemoId))
            {
                MemoList[MemoList.FindIndex(x => x.MemoId == newMemo.MemoId)] = newMemo;
            }
            else MemoList.Add(newMemo);
        }

        public void AddOrUpdateTask(CNTask newTask)
        {
            if (!ValidateNewTask(newTask))
            {
                throw new ArgumentException("Task's Paremeter Is Not Corect!");
            }
            if (TaskList.Exists(r => r.TaskId == newTask.TaskId))
            {
                TaskList[TaskList.FindIndex(x => x.TaskId == newTask.TaskId)] = newTask;
            }
            else TaskList.Add(newTask);
        }

        /// <summary>
        /// 1. Task's pretask & parenttask exists & must not be self
        /// 2. Content is not null
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        private bool ValidateNewTask(CNTask newTask)
        {
            if (string.IsNullOrEmpty(newTask?.Content)) return false;
            if (newTask.PreTaskIds != null && newTask.PreTaskIds.Count > 0)
            {
                if (newTask.PreTaskIds.Exists(x => x == newTask.TaskId))
                    return false;
                if (!newTask.PreTaskIds.TrueForAll(x => TaskList.Exists(r => r.TaskId == x)))
                    return false;
            }
            if (newTask.HasParentTask() && TaskList.All(x => x.TaskId != newTask.ParentTaskId)) return false;
            return true;
        }

        public IEnumerable<CNMemo> GetAllUniqueMemo()
        {
            IEnumerable<CNMemo> memos = new List<CNMemo>();
            TaskList.Where(r=>r.HasMemo()).Select(x=>x.GetAllMemos()).ToList().ForEach(r=>memos = r.Union(memos, CNMemo.CnMemoComparer));
            memos = memos.Union(MemoList,CNMemo.CnMemoComparer);
            return memos;
        }

        public IEnumerable<CNTask> GetAllRootTasks()
        {
            return TaskList.Where(r => !r.HasParentTask()).ToList();
        }

        public IEnumerable<CNMemo> GetTaskMemo(int taskid)
        {
            return (TaskList.FirstOrDefault(x => x.TaskId == taskid) ?? new CNTask()).GetAllMemos();
        }

        public IEnumerable<CNMemo> SearchMemoByContent(string searchContent)
        {
            return GetAllUniqueMemo().Where(r => r.Content.Contains(searchContent) || r.Title.Contains(searchContent));
        }

        public void UpdateMemo(CNMemo updateMemo)
        {
            if (MemoList.Exists(r => r.MemoId == updateMemo.MemoId))
            {
                MemoList[MemoList.FindIndex(x => x.MemoId == updateMemo.MemoId)] = updateMemo;
            }

            TaskList.Where(x=>x.HasMemo(updateMemo.MemoId)).ToList().ForEach(r=>r.AddOrUpdateMemo(updateMemo));
        }

        public void ReplaceAWordOfATaskMemos(string originwords, string targetwords)
        {
            TaskList.ForEach(r => r.ReplaceAWordOfATaskMemos(originwords,targetwords));
            MemoList.ForEach(r=> {
                r.Content = r.Content.Replace(originwords, targetwords);
                r.Title = r.Title.Replace(originwords, targetwords);
            });
        }

        public void StartTask(int taskid)
        {
            (TaskList.FirstOrDefault(r => r.TaskId == taskid) ?? new CNTask()).Start();
        }
        public void StopTask(int taskid)
        {
            (TaskList.FirstOrDefault(r => r.TaskId == taskid) ?? new CNTask()).Stop();
        }

        public void EndTask(int taskid)
        {
            var targetTask = TaskList.FirstOrDefault(r => r.TaskId == taskid);
            if (targetTask == null) return;
            if (targetTask.PreTaskIds != null && targetTask.PreTaskIds.Count > 0)
            {
                var notEndedPreTasks = targetTask.PreTaskIds.Where(x => !string.IsNullOrEmpty(GetTaskById(x).Content)
                                                                        && GetTaskById(x).Status != CNTaskStatus.DONE).ToList();
                if (notEndedPreTasks.Count > 0)
                {
                    throw new PreTaskNotEndedException(notEndedPreTasks.Select(x=>TaskList.First(r=>r.TaskId == x)).ToList(),targetTask);
                }
            }
            targetTask.End();
        }

        public int GetNextTaskID()
        {
            return TaskList.Max(x=>x.TaskId)+1;
        }

        public bool HasChildTasks(int taskId)
        {
            return TaskList.Exists(x=>x.HasParentTask() && x.ParentTaskId == taskId);
        }

        public bool HasSufTasks(int taskId)
        {
            return TaskList.Exists(x => x.PreTaskIds!=null && x.PreTaskIds.Any(r => r == taskId));
        }
        /// <summary>
        /// delete a task,it can be recoverd later
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>how many task has been deleted</returns>
        public int DeleteTaskById(int taskId)
        {
            int count = 0;
            var task = GetTaskById(taskId);
            if (string.IsNullOrEmpty(task?.Content)) return count;
            if (task.IsDeleted) return count;
            TaskList.Where(x => x.HasParentTask() && x.ParentTaskId == taskId).ToList().ForEach(x =>
            {
                count += DeleteTaskById(x.TaskId);
            });
            TaskList.Where(x => x.PreTaskIds != null && x.PreTaskIds.Exists(r => r == taskId)).ToList().ForEach(x =>
            {
                count += DeleteTaskById(x.TaskId);
            });
            task.IsDeleted = true;
            AddOrUpdateTask(task);
            return ++count;
        }
        public int RecoverTaskById(int taskId)
        {
            int count = 0;
            var task = GetTaskById(taskId);
            if (string.IsNullOrEmpty(task?.Content)) return count;
            if (!task.IsDeleted) return count;
            TaskList.Where(x => x.HasParentTask() && x.ParentTaskId == taskId).ToList().ForEach(x =>
            {
                count += RecoverTaskById(x.TaskId);
            });
            TaskList.Where(x => x.PreTaskIds != null && x.PreTaskIds.Exists(r => r == taskId)).ToList().ForEach(x =>
            {
                count += RecoverTaskById(x.TaskId);
            });
            task.IsDeleted = false;
            AddOrUpdateTask(task);
            return ++count;
        }

        /// <summary>
        /// remove a task permenantly
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>how many task has been removed</returns>
        public int RemoveTaskById(int taskId)
        {
            int count = 0;
            var task = GetTaskById(taskId);
            if (string.IsNullOrEmpty(task?.Content)) return count;
            TaskList.Where(x => x.HasParentTask() && x.ParentTaskId == taskId).ToList().ForEach(x =>
            {
                count += RemoveTaskById(x.TaskId);
            });
            TaskList.Where(x => x.PreTaskIds != null && x.PreTaskIds.Exists(r => r == taskId)).ToList().ForEach(x =>
            {
                count += RemoveTaskById(x.TaskId);
            });
            //1 Move tasks memo to global
            task.GetAllMemos().ForEach(AddOrUpdateGlobalMemo);
            //2 Remove current task
            TaskList.Remove(task);
            return ++count;
        }

        public void FailTaskByTaskId(int taskId,string reason)
        {
            var task = GetTaskById(taskId);
            if (string.IsNullOrEmpty(task?.Content)) return;
            task.Stop();
            task.IsFail = true;
            task.FailReason = reason;
        }

        public HashSet<CNTask> GetTaskAndChildSufTasksById(int taskId)
        {
            HashSet<CNTask> tasklist = new HashSet<CNTask>();
            var task = GetTaskById(taskId);
            if (string.IsNullOrEmpty(task?.Content)) return tasklist;
            TaskList.Where(x => x.HasParentTask() && x.ParentTaskId == taskId).ToList().ForEach(x =>
            {
                GetTaskAndChildSufTasksById(x.TaskId).ToList().ForEach(r=> tasklist.Add(r));
            });
            TaskList.Where(x => x.PreTaskIds != null && x.PreTaskIds.Exists(r => r == taskId)).ToList().ForEach(x =>
            {
                GetTaskAndChildSufTasksById(x.TaskId).ToList().ForEach(r => tasklist.Add(r));
            });
            tasklist.Add(task);
            return tasklist;
        }
    }
}
