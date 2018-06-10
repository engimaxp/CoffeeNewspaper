using System;
using System.Drawing;
using Console = Colorful.Console;
namespace CoffeeNewspaper_CLI
{
    public class StartState: BaseState
    {
        public override void InitialCommandList()
        {
            base.InitialCommandList();
            CommandList.Add(new TaskCommand(this));
        }

        public override void Refresh()
        {
            Console.WriteAscii("COFFEE", Color.SandyBrown);
            Console.WriteAscii("    NEWSPAPER", Color.PapayaWhip);
            Console.WriteLine();
            Console.WriteLine("Hello my friend:", Color.YellowGreen);
            Console.WriteLine();
            Console.WriteLineFormatted("  Welcome to use {0}", "CoffeeNewspaper", Color.LightGoldenrodYellow, Color.Gray);
            Console.WriteLine();
            Console.WriteLine("  This is a tool to improve your work efficiency!", Color.Gray);
            Console.WriteLine();
            Console.WriteLineFormatted("  enter {0} to get more information", "help", Color.LightGoldenrodYellow, Color.Gray);
            Console.WriteLine();
        }

        public override void DisplayCmdLine()
        {
            Console.Write("Drink a Coffee> ",Color.White);
        }
    }
}