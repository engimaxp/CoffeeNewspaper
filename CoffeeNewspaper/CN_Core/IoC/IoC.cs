using CN_Core.Interfaces;
using CN_Core.Logging;
using Ninject;

namespace CN_Core
{
    /// <summary>
    ///     The IoC container for our application
    /// </summary>
    public static class IoC
    {
        #region Public Properties

        /// <summary>
        ///     The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; } = new StandardKernel();

        /// <summary>
        ///     A shortcut to access the <see cref="ILogFactory" />
        /// </summary>
        public static ILogFactory Logger => Get<ILogFactory>();

        /// <summary>
        ///     A shortcut to access the <see cref="IFileManager" />
        /// </summary>
        public static IFileManager File => Get<IFileManager>();

        /// <summary>
        ///     A shortcut to access the <see cref="ITaskManager" />
        /// </summary>
        public static ITaskManager Task => Get<ITaskManager>();

        #endregion

        /// <summary>
        ///     Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        #region Construction

        /// <summary>
        ///     Sets up the IoC container, binds all information required and is ready for use
        ///     NOTE: Must be called as soon as your application starts up to ensure all
        ///     services can be found
        /// </summary>
        public static void Setup()
        {
            // Bind all required view models
            BindViewModels();
        }

        /// <summary>
        ///     Binds all singleton view models
        /// </summary>
        private static void BindViewModels()
        {
            // Add our file logger factory
            if(Kernel.TryGet<ILogFactory>()==null)
            Kernel.Bind<ILogFactory>().ToConstant(new BaseLogFactory());
            // Add our task manager
            if (Kernel.TryGet<ITaskManager>() == null)
                Kernel.Bind<ITaskManager>().ToConstant(new TaskManager());

            if (Kernel.TryGet<IFileManager>() == null)
                Kernel.Bind<IFileManager>().ToConstant(new FileManager());
        }

        #endregion
    }
}