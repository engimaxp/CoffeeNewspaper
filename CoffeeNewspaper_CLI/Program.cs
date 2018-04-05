using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoffeeNewspaper_CLI.ConsoleTables;

namespace CoffeeNewspaper_CLI
{
    class Program
    {
        public class Something
        {
            public Something()
            {
                Id = Guid.NewGuid().ToString("N");
                Name = "Khalid Abuhkameh";
                Date = DateTime.Now;
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
        }

        static void Main(string[] args)
        {
            var table = new ConsoleTable("one", "two", "three");
            table.AddRow(1, 2, 3)
                .AddRow("this line should be longer", "yes it is", "oh");

            table.Write();
            Console.WriteLine();

            var rows = Enumerable.Repeat(new Something(), 10);

            ConsoleTable
                .From<Something>(rows)
                .Write(Format.Alternative);

            Console.ReadKey();
        }
    }
}
