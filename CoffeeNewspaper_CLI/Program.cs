using System;
using System.Threading.Tasks;
using CN_Core;
using CN_Repository;
using CN_Service;

namespace CoffeeNewspaper_CLI
{
    class Program
    {
        /// <summary>
        ///     Application entry point
        /// </summary>
        /// <param name="args">Command line arguments</param>
        private static void Main(string[] args)
        {
            //Ioc Setup
            IoC.Setup();
            IoC.Kernel.BindCNDBContext();
            IoC.Kernel.BindSqliteDataStore();
            IoC.Kernel.BindServices();

            // enable ctrl+c
            Console.CancelKeyPress += (o, e) =>
            {
                Environment.Exit(1);
            };
            BaseState currentStatue = new StartState();
            currentStatue.Display(null);
            while (true)
            {
                currentStatue.InitialCommandList();
                currentStatue.DisplayCmdLine();
                string input = Console.ReadLine();
                if ("exit".Equals(input))
                {
                    break;
                }
                ArgumentParser parser = new ArgumentParser(input);
                var statue = currentStatue;
                var task = Task.Run(async () => await statue.DoCommand(parser));
                task.Wait();
                var nextState =task.Result;
                if (nextState != currentStatue)
                {
                    nextState.Display(currentStatue);
                    currentStatue = nextState;
                }
            }
        }
    }
}
