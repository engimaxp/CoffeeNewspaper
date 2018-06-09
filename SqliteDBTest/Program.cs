using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
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
            IoC.Setup();
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

                    using (var context = CNDbContext.GetFileSqlDatabase())
                    {
                        context.Database.OpenConnection();
                        context.Database.EnsureCreated();

                        Console.WriteLine("Add a Task");

                        var taskDataStore = new TaskDataStore(context);
                        var assesTask = new CNTask
                        {
                            Content = "Write A Program" + DateTime.Now + Guid.NewGuid().ToString("N"),
                            CreateTime = DateTime.Now,
                            StartTime = null,
                            EndTime = null,
                            Priority = CNPriority.High,
                            Urgency = CNUrgency.High,
                            Status = CNTaskStatus.TODO,
                            EstimatedDuration = 3600
                        };
                        var sufTask = new CNTask
                        {
                            Content = "Write A Program" + DateTime.Now + Guid.NewGuid().ToString("N"),
                            CreateTime = DateTime.Now,
                            StartTime = null,
                            EndTime = null,
                            Priority = CNPriority.High,
                            Urgency = CNUrgency.High,
                            Status = CNTaskStatus.TODO,
                            EstimatedDuration = 3600
                        };
                        assesTask.SufTaskConnectors.Add(new CNTaskConnector(){PreTask = assesTask,SufTask =sufTask });
                        await taskDataStore.AddTask(assesTask);


                        assesTask.SufTaskConnectors.Add(new CNTaskConnector() { PreTaskId = assesTask.TaskId, SufTaskId = assesTask.SufTaskConnectors.First().SufTaskId });
                        await taskDataStore.UpdateTask(assesTask);

                        Console.WriteLine("Query all Task");
                        var tasks = await taskDataStore.GetAllTask();
                        tasks.ToList().ForEach(x =>
                        {
                            Console.WriteLine("Task:");
                            Console.WriteLine(x);


                            Console.WriteLine("Distinct PreTasks:");
                            x.PreTaskConnectors.Distinct().ToList().ForEach(Console.WriteLine);
                            Console.WriteLine("Distinct SufTasks:");
                            x.SufTaskConnectors.Distinct().ToList().ForEach(Console.WriteLine);

                            Console.WriteLine("PreTasks:");
                            x.PreTaskConnectors.ToList().Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).ToList().ForEach(Console.WriteLine);
                            Console.WriteLine("SufTasks:");
                            x.SufTaskConnectors.ToList().Distinct(CNTaskConnector.PreTaskIdSufTaskIdComparer).ToList().ForEach(Console.WriteLine);
                        });
                        context.Database.CloseConnection();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
            task.Wait();
            Console.WriteLine("Finishied");
            Console.ReadLine();
        }
    }
}