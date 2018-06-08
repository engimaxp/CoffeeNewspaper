using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CN_Core;
using CN_Core.Interfaces.Service;
using CN_Service;
using CoffeeNewspaper_CLI.ConsoleTables;
using Console = Colorful.Console;
namespace CoffeeNewspaper_CLI
{
    public class TaskEditState : BaseState
    {
        public override void InitialCommandList()
        {
            base.InitialCommandList();
            CommandList.Add(new TaskEditCommand(this));
            CommandList.Add(new TagCommand(this));
            CommandList.Add(new SaveTaskCommand(this));
        }

        public override void Refresh()
        {
            if (StateObj == null) StateObj = new CNTask();
            if (StateObj is CNTask task)
            {
                Console.WriteAscii(task.TaskId == 0?"New Task":"Edit Task",
                    SaveTaskCommand.ValidInputComplete(task) ? Color.LawnGreen : Color.SandyBrown);

                Console.WriteLine();
                if (task.IsFail)
                {
                    Console.WriteLine($"{"IsFail:",-20}{task.IsFail}");
                    Console.WriteLine($"{"FailReason:",-20}{task.FailReason}");
                }

                if (task.IsDeleted)
                {
                    Console.WriteLine($"{"IsDeleted:",-20}{task.IsDeleted}");
                }
                Console.WriteLine($"{"Content:",-20}{task.Content}");
                Console.WriteLine($"{"DeadLine:",-20}{task.DeadLine}");
                Console.WriteLine($"{"EstimatedDuration:",-20}{DisplayDuration(task.EstimatedDuration)}");
                Console.WriteLine($"{"Priority",-20}{Enum.GetName(typeof(CNPriority), task.Priority)}");
                Console.WriteLine($"{"Urgency:",-20}{Enum.GetName(typeof(CNUrgency), task.Urgency)}");
                Console.WriteLine($"{"Tags:",-20}{string.Join(",",task.TaskTaggers.ToList().Select(x=>x.Tag).Select(y=>y.Title))}");
                Console.WriteLine($"{"CreateTime:",-20}{(task.CreateTime > DateTime.MinValue?task.CreateTime:DateTime.Now)}");
                Console.WriteLine($"{"StartTime:",-20}{task.StartTime}");
                Console.WriteLine($"{"EndTime:",-20}{task.EndTime}");
                Console.WriteLine($"{"Memos:",-20}{task.TaskMemos.Select(x=>x.Memo).ToList().Count + "memos"}");

                if (!string.IsNullOrEmpty(task.ParentTask?.Content))
                {
                        Console.WriteLine($"{"Parent:",-20}({task.ParentTask.TaskId}){task.ParentTask.Content}");
                }

                if (task.PreTaskConnectors.Any())
                {
                    Console.WriteLine($"{"PreTaskIds:",-20}");
                    var preTasks = task.PreTaskConnectors.Select(x=>x.PreTask);
                    ConsoleTable tables = ConsoleTable.From(preTasks.Select(x => new { TaskID = x.TaskId, Content = x.Content }));
                    tables.WriteAlternateColorTable(Color.LightGoldenrodYellow, Color.White);
                }

                Console.WriteLine($"{"Status:",-20}{Enum.GetName(typeof(CNTaskStatus), task.Status)}");
            }
            else
            {
                Console.WriteLine("StateObj wrong , please restart!");
            }
            Console.WriteLine();
        }

        public override void DisplayCmdLine()
        {
            if (StateObj is CNTask task)
            {
                Console.Write((task.TaskId == 0 ? "New Task" : "Edit Task") + "> ", Color.White);
            }
            else
            {
                Console.Write("Drink a Coffee> ", Color.White);
            }
        }

        private string DisplayDuration(int estimatedDurationMinutes)
        {
            var minutes = estimatedDurationMinutes % 60;
            int remains = (estimatedDurationMinutes - minutes) / 60;
            var hours = remains % 24;
            remains = (remains - hours) / 24;
            var days = remains % 7;
            var weeks = (remains - days) / 7;
            var result = 
                $"{(weeks > 0 ? (weeks + "w") : "")}{(days > 0 ? (days + "d") : "")}{(hours > 0 ? (hours + "h") : "")}";
            if (string.IsNullOrEmpty(result) || minutes > 0)
            {
                result += minutes + "m";
            }

            return result;
        }
    }
}