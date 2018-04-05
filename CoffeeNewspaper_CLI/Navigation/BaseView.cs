using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.Linq;
using System.Text;
namespace CoffeeNewspaper_CLI.Navigation
{
    public class BaseView
    {
        private string _consoleTitle;

        private string _currentOutput;

        public BaseView()
        {
//            Console.BufferHeight = Console.WindowHeight;
            ConsoleTitle = "New Window";
            InputCursorPosition = new Tuple<int, int>(0, Console.WindowHeight - 2);
            OutputCursorPosition = new Tuple<int, int>(0, 0);
            StatusText = "Here is a default window!";
            _currentOutput = "This is a Test window , you can write at the bottom!";
        }

        public string ConsoleTitle
        {
            get => _consoleTitle;
            set
            {
                Console.Title = value;
                _consoleTitle = value;
            }
        }

        public Tuple<int, int> InputCursorPosition { get; set; }

        public Tuple<int, int> OutputCursorPosition { get; set; }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set
            {
                //clear status bar
                Console.SetCursorPosition(2, Console.WindowHeight - 1);
                Console.Write(string.Format("{0," + (Console.WindowWidth - 3) + "}", " "));
                //set status bar
                Console.SetCursorPosition(2, Console.WindowHeight-1);
                Console.Write(value);
                //reset cursor
                Console.SetCursorPosition(InputCursorPosition.Item1, InputCursorPosition.Item2);
                _statusText = value;
            }
        }
        protected virtual void ResetCursor()
        {
            InputCursorPosition = new Tuple<int, int>(0, Console.WindowHeight - 2);
            OutputCursorPosition = new Tuple<int, int>(0, 0);
        }

        protected virtual void Write(string output)
        {
            Console.SetCursorPosition(OutputCursorPosition.Item1, OutputCursorPosition.Item2);
            Console.WriteLine(output);
            OutputCursorPosition = new Tuple<int, int>(Console.CursorLeft,Console.CursorTop);
            Console.SetCursorPosition(InputCursorPosition.Item1, InputCursorPosition.Item2);
        }

        protected virtual void WaitRead()
        {
            Console.SetCursorPosition(InputCursorPosition.Item1, InputCursorPosition.Item2);
            string input = null;
            do
            {
                input = Console.ReadLine();
                ClearInputArea();
                if ("refresh".Equals(input))
                {
                    Display();
                }
                else if (string.IsNullOrEmpty(input))
                {
                    ClearInputArea();
                }
                else if (input.StartsWith("status"))
                {
                    StatusText = input.Replace("status", "").Trim();
                }
                else if (input.Contains("exit"))
                {
                    break;
                }
                else
                {
                    Write(input);
                    _currentOutput += Environment.NewLine+ input;
                }
            }
            while (!(input!=null && input.Contains("exit"))) ;
        }

        protected virtual void ClearInputArea()
        {
            Console.SetCursorPosition(InputCursorPosition.Item1, InputCursorPosition.Item2);
            Console.Write("{0," + (Console.WindowWidth) + "}", " ");
            Console.SetCursorPosition(InputCursorPosition.Item1, InputCursorPosition.Item2);
        }

        public virtual void Display()
        {
            Console.Clear();
            Console.BufferHeight = Console.WindowHeight;
            ResetCursor();
            ConsoleTitle = _consoleTitle;
            StatusText = _statusText;
            Write(_currentOutput);
            WaitRead();
        }
    }
}
