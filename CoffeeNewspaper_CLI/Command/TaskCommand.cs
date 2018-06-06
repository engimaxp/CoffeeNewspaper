using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Service;
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

        public override async Task<BaseState> Excute(ArgumentParser input)
        {
            if (input.Defined("add"))
            {
                return new TaskEditState();
            }
            if (input.Defined("l"))
            {
                var lst = await IoC.Get<ITaskService>().GetAllTasks();
                ConsoleTable tables = ConsoleTable.From(lst.Select(x => new {TaskID = x.TaskId, Content = x.Content}));
                tables.WriteAlternateColorTable(Color.LightGoldenrodYellow, Color.White);
                Console.WriteLine();
                return State;
            }
            if (input.Defined("more"))
            {
                var taskid = input.GetSingle<int>("more");
                var state = new TaskEditState {StateObj = await IoC.Get<ITaskService>().GetTaskById(taskid)};
                return state;
            }

            Console.WriteLine("wrong parameter", Color.Red);
            Console.WriteLine();
            return State;
        }
    }
}