using System;
using System.Collections.Generic;
using System.Drawing;
using Console = Colorful.Console;
namespace CoffeeNewspaper_CLI
{
    public class BaseState
    {

        protected HashSet<BaseCommand> CommandList = new HashSet<BaseCommand>();

        public BaseState LastState { get; private set; }

        public object StateObj { get;set; }

        public virtual void InitialCommandList()
        {
            CommandList.Add(new HelpCommand(this));
            CommandList.Add(new BackCommand(this));
        }

        public BaseState DoCommand(ArgumentParser input)
        {
            if (string.IsNullOrEmpty(input.CommandLine)) return this;
            Console.WriteLine();
            foreach (var command in CommandList)
            {
                if (command.Name.Equals(input.CommandLine, StringComparison.CurrentCultureIgnoreCase))
                {
                    return command.Excute(input);
                }
            }
            Console.WriteLineFormatted("{0} is Not a valid command", input.CommandLine, Color.LightGoldenrodYellow, Color.Gray);
            Console.WriteLine();
            return this;
        }

        public void Display(BaseState lastState)
        {
            LastState = lastState;
            Refresh();
        }

        public virtual void Refresh()
        {
            throw new NotImplementedException();
        }

        public virtual void DisplayCmdLine()
        {
            throw new NotImplementedException();
        }
    }
}
