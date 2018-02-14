using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN_Model;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    public static class DomainTestHelper
    {
        public static CNTask GetARandomTask(int taskid)
        {
            string timestamp = DateTime.Now.ToString("s");
            return new CNTask()
            {
                TaskId = taskid,
                Content = "Write A Program" + timestamp + Guid.NewGuid().ToString("N"),
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                EstimatedDuration = 3600,
                Tags = new List<string>() {"Work"},
            };
        }

        public static CNMemo GetARandomMemo(int memoid)
        {
            string timestamp = DateTime.Now.ToString("s");
            return new CNMemo()
            {
                MemoId = memoid,
                Content = "Start with writing tests!" + timestamp + Guid.NewGuid().ToString("N"),
                Tag = ""
            };
        }
    }
}
