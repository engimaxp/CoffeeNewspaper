using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Service;
using Ninject.MockingKernel.NSubstitute;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.ServiceTest
{
    public class ServiceSetupTearDown
    {
        protected readonly NSubstituteMockingKernel _kernel;
        public ServiceSetupTearDown()
        {
            _kernel = new NSubstituteMockingKernel();
            _kernel.Bind<ITaskService>().To<TaskService>();
            _kernel.Bind<IMemoService>().To<MemoService>();
        }
        [SetUp]
        public void SetUp()
        {
            _kernel.Reset();
        }
    }
}
