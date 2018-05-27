using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using CoffeeNewspaper_CLI.ConsoleTables;

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
                var nextState = currentStatue.DoCommand(parser);
                if (nextState != currentStatue)
                {
                    nextState.Display(currentStatue);
                    currentStatue = nextState;
                }
            }
        }
        
    }
}
