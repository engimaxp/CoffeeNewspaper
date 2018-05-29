using Ninject;

namespace CN_Service
{
    public static class ServiceImplementBinder
    {
        /// <summary>
        /// Bind ioc to actual service
        /// </summary>
        /// <param name="Kernel"></param>
        public static void BindServices(this IKernel Kernel)
        {
            Kernel.Bind<IMemoService>().To<MemoService>();

            Kernel.Bind<ITaskService>().To<TaskService>();
            
        }
    }
}
