using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoffeeNewspaper_CLI.ConsoleTables;
using CoffeeNewspaper_CLI.Navigation;
namespace CoffeeNewspaper_CLI
{
    class Program
    {
//        public class Something
//        {
//            public Something()
//            {
//                Id = Guid.NewGuid().ToString("N");
//                Name = "Khalid Abuhkameh";
//                Date = DateTime.Now;
//            }
//
//            public string Id { get; set; }
//            public string Name { get; set; }
//            public DateTime Date { get; set; }
//        }
//
//        protected static int origRow;
//        protected static int origCol;
//
//        protected static void WriteAt(string s, int x, int y)
//        {
//            try
//            {
//                Console.SetCursorPosition(origCol + x, origRow + y);
//                Console.Write(s);
//            }
//            catch (ArgumentOutOfRangeException e)
//            {
//                Console.Clear();
//                Console.WriteLine(e.Message);
//            }
//        }
        static void Main(string[] args)
        {
//            Console.TreatControlCAsInput = true;
            Nav.GetNavigation().Goto(new BaseView());
//            Console.Title = "Testing!";
//            Console.TreatControlCAsInput = true;
//            // Clear the screen, then save the top and left coordinates.
//            Console.Clear();
//            origRow = Console.CursorTop;
//            origCol = Console.CursorLeft;
//
//            // Draw the left side of a 5x5 rectangle, from top to bottom.
//            WriteAt("+", 0, 0);
//            WriteAt("|", 0, 1);
//            Console.ReadLine();
//            WriteAt("|", 0, 2);
//            WriteAt("|", 0, 3);
//            WriteAt("+", 0, 4);
//
//            // Draw the bottom side, from left to right.
//            WriteAt("-", 1, 4); // shortcut: WriteAt("---", 1, 4)
//            WriteAt("-", 2, 4); // ...
//            WriteAt("-", 3, 4); // ...
//            WriteAt("+", 4, 4);
//
//            // Draw the right side, from bottom to top.
//            WriteAt("|", 4, 3);
//            WriteAt("|", 4, 2);
//            Console.ReadLine();
//            WriteAt("|", 4, 1);
//            WriteAt("+", 4, 0);
//
//            // Draw the top side, from right to left.
//            WriteAt("-", 3, 0); // shortcut: WriteAt("---", 1, 0)
//            WriteAt("-", 2, 0); // ...
//            Console.SetCursorPosition(0, Console.WindowHeight - 1);
//            Console.ReadLine();
//            WriteAt("-", 1, 0); // ...
//            //
//            WriteAt("All done!", 0, 6);
//            Console.WriteLine();
//            Console.ReadKey();
            //            var table = new ConsoleTable("one", "two", "three");
            //            table.AddRow(1, 2, 3)
            //                .AddRow("this line should be longer", "yes it is", "oh");
            //
            //            table.Write();
            //            Console.WriteLine();
            //
            //            Console.ReadKey();
            //            Console.Clear();
            //            var rows = Enumerable.Repeat(new Something(), 10);
            //
            //            ConsoleTable
            //                .From<Something>(rows)
            //                .Write(Format.Minimal);
            //
            //            Console.ReadKey();
        }
    }
}
