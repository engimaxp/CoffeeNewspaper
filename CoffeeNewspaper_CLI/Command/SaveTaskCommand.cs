using System;
using System.Drawing;
using CN_BLL;
using CN_Model;
using Console = Colorful.Console;

namespace CoffeeNewspaper_CLI
{
    public class SaveTaskCommand : BaseCommand
    {
        public SaveTaskCommand(BaseState state) : base(state)
        {
            Name = "save";
        }

        public override BaseState Excute(ArgumentParser input)
        {
            if (State != null && State.StateObj is CNTask task)
            {
                if (!ValidInputComplete(task,enableConsoleOutput:true))
                {
                    Console.WriteLine("edit not complete", Color.Red);
                    return State;
                }

                if (task.TaskId == 0)
                {
                    task.CreateTime = DateTime.Now;
                    if (new TaskService().CreateATask(task) > 0)
                    {
                        Console.WriteLine("task create success!", Color.Green);
                    }
                    else
                    {
                        Console.WriteLine("task create fail!", Color.Red);
                    }
                }
                else
                {
                    if (new TaskService().EditATask(task))
                    {
                        Console.WriteLine("task edit success!", Color.Green);
                    }
                    else
                    {
                        Console.WriteLine("task edit fail!", Color.Red);
                    }
                }

            }
            else
            {
                Console.WriteLine("wrong parameter",Color.Red);
            }

            Console.WriteLine();
            return new StartState();
        }

        public static bool ValidInputComplete(CNTask task,bool enableConsoleOutput = false)
        {
            bool result = true;
            if (string.IsNullOrEmpty(task.Content))
            {
                if(enableConsoleOutput)
                    Console.WriteLine("Content should not be empty! ",Color.Yellow);
                result = false;
            }

            if (task.Priority == 0)
            {
                if (enableConsoleOutput)
                    Console.WriteLine("Priority should not be empty! ", Color.Yellow);
                result = false;
            }
            if (task.Urgency == 0)
            {
                if (enableConsoleOutput)
                    Console.WriteLine("Urgency should not be empty! ", Color.Yellow);
                result = false;
            }
            return result;
        }
    }
}