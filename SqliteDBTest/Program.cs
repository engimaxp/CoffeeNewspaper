using System;
using System.IO;
using System.Threading.Tasks;
using CN_Core.IoC;
using CN_Repository;

namespace SqliteDBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                File.Delete(CNDbContext.dbfilename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Hello World!");
            //Bind the Ioc
            IoC.Kernel.BindCNDBContext();

            // Ensure the Database is Created
            var task = Task.Run(async () =>
            {
                try
                {
                    var dbContext = IoC.Get<CNDbContext>();
                    await dbContext.Database.EnsureCreatedAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            task.Wait();
            Console.WriteLine("Finishied");
            Console.ReadLine();
        }
    }
}
