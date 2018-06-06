using CN_Core.Interfaces.Repository;
using CN_Repository;
using Ninject;

namespace CN_Service
{
    public static class DataStoreImplementBinder
    {
        /// <summary>
        /// Bind ioc to actual datastore
        /// </summary>
        /// <param name="Kernel"></param>
        public static void BindSqliteDataStore(this IKernel Kernel)
        {
            Kernel.Bind<IMemoDataStore>().To<MemoDataStore>();
            Kernel.Bind<ITimeSliceDataStore>().To<TimeSliceDataStore>();
            Kernel.Bind<ITaskDataStore>().To<TaskDataStore>();
        }
    }
}
