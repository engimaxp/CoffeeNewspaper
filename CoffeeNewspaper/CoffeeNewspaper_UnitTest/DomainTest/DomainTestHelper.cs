using System;
using CN_Core;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    public static class DomainTestHelper
    {
        public static CNTask GetARandomTask()
        {
            string timestamp = DateTime.Now.ToString("s");
            return new CNTask()
            {
                Content = "Write A Program" + timestamp + Guid.NewGuid().ToString("N"),
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                Status = CNTaskStatus.TODO,
                EstimatedDuration = 3600,
            };
        }

        public static CNMemo GetARandomMemo(string memoid)
        {
            string timestamp = DateTime.Now.ToString("s");
            return new CNMemo()
            {
                MemoId = memoid,
                Title = "NewMemo",
                Content = "Start with writing tests!" + timestamp + Guid.NewGuid().ToString("N"),
            };
        }

        public static CNTag GetARandomTag()
        {
            string timestamp = DateTime.Now.ToString("s");
            return new CNTag()
            {
                Title = $"randomtag{timestamp}"
            };
        }
    }
}
