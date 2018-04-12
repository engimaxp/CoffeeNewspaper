using Colorful;

namespace CoffeeNewspaper_CLI
{
    public class BackCommand : BaseCommand
    {
        public BackCommand(BaseState state) : base(state)
        {
            Name = "back";
        }

        public override BaseState Excute(ArgumentParser input)
        {
            if (State.LastState == null)
            {
                Console.WriteLine("you have already back to the begining !");
                Console.WriteLine();
                return State;
            }
            else
            {
                Console.WriteLine();
                return State.LastState;
            }
        }
    }
}