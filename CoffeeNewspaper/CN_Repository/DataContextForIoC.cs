﻿using Ninject;

namespace CN_Repository
{
    /// <summary>
    ///     Extension method to inject CNDBContext to Container
    /// </summary>
    public static class DataContextForIoC
    {
        /// <summary>
        ///     Bind the DbContext
        ///     shall do this at the beginning of app start
        /// </summary>
        /// <param name="Kernel"></param>
        /// <param name="InMemoryMode">true if used in unittest</param>
        public static void BindCNDBContext(this IKernel Kernel, bool InMemoryMode = false)
        {
            if (Kernel == null) return;
            var origindbContext = Kernel.TryGet<CNDbContext>();
            // if the originDBContext exists then jump to exit
            if (origindbContext != null) return;

            // create a default dbContext object
            var dbContext = InMemoryMode ? CNDbContext.GetMemorySqlDatabase() : CNDbContext.GetFileSqlDatabase();

            // bind a default DBContext to IoC
            Kernel.Bind<CNDbContext>().ToConstant(dbContext);
        }

        /// <summary>
        ///     Unbind the DbContext
        ///     this can be used while app tear down
        /// </summary>
        /// <param name="Kernel"></param>
        public static void UnBindCNDBContext(this IKernel Kernel)
        {
            if (Kernel == null) return;
            var origindbContext = Kernel.TryGet<CNDbContext>();
            // if the originDBContext exists then jump to exit
            if (origindbContext != null) return;
            Kernel.Unbind<CNDbContext>();
        }
    }
}