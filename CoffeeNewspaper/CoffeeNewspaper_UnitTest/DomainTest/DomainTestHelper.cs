using System;
using CN_Core;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    public static class DomainTestHelper
    {
        /// <summary>
        ///     Get a random task
        /// </summary>
        /// <returns></returns>
        public static CNTask GetARandomTask(int id = 0)
        {
            var timestamp = DateTime.Now.ToString("s");
            return new CNTask
            {
                TaskId = id,
                Content = "Write A Program" + timestamp + Guid.NewGuid().ToString("N"),
                CreateTime = DateTime.Now,
                StartTime = null,
                EndTime = null,
                Priority = CNPriority.High,
                Urgency = CNUrgency.High,
                Status = CNTaskStatus.TODO,
                EstimatedDuration = 3600
            };
        }

        /// <summary>
        ///     Get a random memo
        /// </summary>
        /// <returns></returns>
        public static CNMemo GetARandomMemo(bool generateId = false)
        {
            var timestamp = DateTime.Now.ToString("s");
            return new CNMemo
            {
                MemoId = generateId? Guid.NewGuid().ToString("D") :null,
                Title = "NewMemo",
                Content = "Start with writing tests!" + timestamp + Guid.NewGuid().ToString("N")
            };
        }

        /// <summary>
        ///     Get a random tag
        /// </summary>
        /// <returns></returns>
        public static CNTag GetARandomTag()
        {
            var timestamp = DateTime.Now.ToString("s");
            return new CNTag
            {
                Title = $"randomtag{timestamp}{Guid.NewGuid():D}"
            };
        }

        /// <summary>
        ///     Get a random timeslice
        /// </summary>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public static CNTimeSlice GetARandomTimeSlice(DateTime? EndTime = null)
        {
            return new CNTimeSlice(DateTime.Now, null);
        }
    }
}