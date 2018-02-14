using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CN_Model
{
    public class CNRoot
    {
        public CNRoot()
        {
            _taskList = new List<CNTask>();
            _memoList = new List<CNMemo>();
        }

        private List<CNTask> TaskList
        {
            get { return _taskList; }
        }

        private List<CNMemo> _memoList;
        private readonly List<CNTask> _taskList;

        private List<CNMemo> MemoList
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

        public CNMemo GetMemoById(int memoid)
        {
            var globalMemo = MemoList.FirstOrDefault(r => r.MemoId == memoid);
            if (globalMemo != null)
            {
                return globalMemo;
            }
            return (TaskList.Select(x => x.GetAllMemos()).Where(r => r != null && r.Count > 0).ToList().FirstOrDefault(x => x.FirstOrDefault(r => r.MemoId == memoid) != null)??new List<CNMemo>()).FirstOrDefault(t => t.MemoId == memoid);
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
            if (TaskList.Exists(r => r.TaskId == newTask.TaskId))
            {
                TaskList[TaskList.FindIndex(x => x.TaskId == newTask.TaskId)] = newTask;
            }
            else TaskList.Add(newTask);
        }

        public IEnumerable<CNMemo> GetAllUniqueMemo()
        {
            IEnumerable<CNMemo> memos = new List<CNMemo>();
            TaskList.Where(r=>r.HasMemo()).Select(x=>x.GetAllMemos()).ToList().ForEach(r=>memos = r.Union(memos));
            memos = memos.Union(this.MemoList);
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
            return this.GetAllUniqueMemo().Where(r => r.Content.Contains(searchContent));
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
            this.MemoList.ForEach(r=>r.Content = r.Content.Replace(originwords,targetwords));
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
            (TaskList.FirstOrDefault(r => r.TaskId == taskid) ?? new CNTask()).End();
        }
    }
}
