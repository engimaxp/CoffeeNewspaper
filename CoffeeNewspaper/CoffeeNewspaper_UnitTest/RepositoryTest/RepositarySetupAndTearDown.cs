using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.IoC;
using CN_Repository;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.RepositoryTest
{
    public class RepositarySetupAndTearDown
    {
        [SetUp]
        public void Setup()
        {
            //Bind the Ioc
            IoC.Kernel.BindCNDBContext();

            // Ensure the Database is Created
            var task = Task.Run(async () =>
            {
                try
                {
                    var dbContext = IoC.Get<CNDbContext>();
                    return await dbContext.Database.EnsureCreatedAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            task.Wait();
        }

        [TearDown]
        public void TearDown()
        {
            //Unbind the Ioc
//            IoC.Kernel.UnBindCNDBContext();

            //Delete the databaase file
//            FileHelper.DumpFile(CNDbContext.dbfilename);
        }
    }
}