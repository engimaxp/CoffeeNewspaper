using CN_Core;
using CN_Core.Interfaces.Service;
using Ninject;
using Ninject.Extensions.NamedScope;

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
            Kernel.Bind<IMemoService>().To<MemoService>().DefinesNamedScope(CNConstants.NinjectNamedScopeForService);

            Kernel.Bind<ITaskService>().To<TaskService>().DefinesNamedScope(CNConstants.NinjectNamedScopeForService);

            Kernel.Bind<ITagService>().To<TagService>().DefinesNamedScope(CNConstants.NinjectNamedScopeForService);
        }
    }
}
