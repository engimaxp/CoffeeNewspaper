using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.IoC;
using CN_Repository;
using Microsoft.EntityFrameworkCore;

namespace SqliteDBTest
{
    internal class Program
    {
        private static void Main(string[] args)
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
            IoC.Kernel.BindCNDBContext(InMemoryMode: true);

            // Ensure the Database is Created
            var task = Task.Run(async () =>
            {
                try
                {
//                    var dbContext = IoC.Get<CNDbContext>();
//                    await dbContext.Database.EnsureCreatedAsync();

                    using (var context = CNDbContext.GetMemorySqlDatabase())
                    {
                        context.Database.OpenConnection();
                        context.Database.EnsureCreated();

                        Console.WriteLine("Add a Task");

                        var taskDataStore = new TaskDataStore(context);

                        await taskDataStore.AddTask(new CNTask
                        {
                            Content = "Write A Program" + DateTime.Now + Guid.NewGuid().ToString("N"),
                            CreateTime = DateTime.Now,
                            StartTime = null,
                            EndTime = null,
                            Priority = CNPriority.High,
                            Urgency = CNUrgency.High,
                            Status = CNTaskStatus.TODO,
                            EstimatedDuration = 3600
                        });

                        Console.WriteLine("Query all Task");
                        var tasks = await taskDataStore.GetAllTask();
                        tasks.ToList().ForEach(Console.WriteLine);
                        context.Database.CloseConnection();
                    }
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