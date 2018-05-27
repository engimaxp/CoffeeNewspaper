using System.Drawing;
using System.Linq;
using CN_BLL;
using CoffeeNewspaper_CLI.ConsoleTables;
using Colorful;

namespace CoffeeNewspaper_CLI
{
    public class TaskCommand : BaseCommand
    {
        public TaskCommand(BaseState state) : base(state)
        {
            Name = "task";
        }

        public override BaseState Excute(ArgumentParser input)
        {
            if (input.Defined("add"))
            {
                return new TaskEditState();
            }
            if (input.Defined("l"))
            {
                var lst = new TaskService().GetAllTasks();
                ConsoleTable tables = ConsoleTable.From(lst.Select(x => new {TaskID = x.TaskId, Content = x.Content}));
                tables.WriteAlternateColorTable(Color.LightGoldenrodYellow, Color.White);
                Console.WriteLine();
                return State;
            }
            if (input.Defined("more"))
            {
                var taskid = input.GetSingle<int>("more");
                var state = new TaskEditState {StateObj = new TaskService().GetTaskById(taskid)};
                return state;
            }

            Console.WriteLine("wrong parameter", Color.Red);
            Console.WriteLine();
            return State;
        }
    }
}