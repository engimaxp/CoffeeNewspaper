using System.Drawing;
using CoffeeNewspaper_CLI.ConsoleTables;
using Colorful;

namespace CoffeeNewspaper_CLI
{
    public class HelpCommand : BaseCommand
    {
        public HelpCommand(BaseState state) : base(state)
        {
            Name = "help";
        }

        public override BaseState Excute(ArgumentParser input)
        {
            var table = new ConsoleTable("Command", "Param", "Description");

            if (State is TaskEditState)
            {
                //edit
                table
                    .AddRow("edit", "-c", "edit content")
                    .AddRow("", "-d", "edit deadline")
                    .AddRow("", "-e", "edit estimatedDuration")
                    .AddRow("", "--parent", "edit parentTask")
                    .AddRow("", "-p", "edit priority level 1(least) 2 3 4 5(most)")
                    .AddRow("", "-u", "edit urgency level 1(least) 2 3 4 5(most)");
                //tag
                table
                    .AddRow("tag", "--add", "add tag")
                    .AddRow("", "--remove", "remove tag");
                //TODO memos
                table
                    .AddRow("memo", "-l", "show all global memos")
                    .AddRow("", "--filter", "set filters")
                    .AddRow("", "--add", "jump to add memo view")
                    .AddRow("", "--delete", "delete a memo")
                    .AddRow("", "--more", "show memo detail view");
                //TODO find
                table.AddRow("find", "", "find all tasks with content");

                //TODO pretask
                table.AddRow("pre", "--add", "add pretask by ids").AddRow("", "--remove", "remove pretask by ids");
                //save
                table
                    .AddRow("save", "", "save and exit");
            }
            else
            {
                //tasks
                table
                    .AddRow("task", "-l", "show all tasks")
                    .AddRow("", "--add", "jump to add task view")
                    .AddRow("", "--delete", "delete a task (none-force mode)")
                    .AddRow("", "--forcedelete", "delete a task (force mode)")
                    .AddRow("", "--more", "show tasks detail view")
                    .AddRow("", "--start", "start a task")
                    .AddRow("", "--stop", "stop a task")
                    .AddRow("", "--end", "finish a task");
                //divider
                table.AddRow("", "", "");
                //TODO memos
                table
                    .AddRow("memo", "-l", "show all global memos")
                    .AddRow("", "--filter", "set filters")
                    .AddRow("", "--add", "jump to add memo view")
                    .AddRow("", "--delete", "delete a memo")
                    .AddRow("", "--more", "show memo detail view");

            }

            table.AddRow("", "", "");
            //utils
            table.AddRow("help", "", "get avalible commands of current state");
            table.AddRow("back", "", "back to last view");

            table.WriteAlternateColorTable(Color.Yellow, Color.LawnGreen);
            Console.WriteLine();
            return State;
        }
    }
}