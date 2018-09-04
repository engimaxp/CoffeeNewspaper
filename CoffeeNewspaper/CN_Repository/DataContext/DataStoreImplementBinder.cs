using CN_Core.Interfaces;
using CN_Core.Interfaces.Repository;
using Ninject;

namespace CN_Repository
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
            Kernel.Bind<ITagDataStore>().To<TagDataStore>();

            Kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
        }
    }
}
