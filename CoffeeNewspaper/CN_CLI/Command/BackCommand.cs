using System.Threading.Tasks;
using Colorful;

namespace CoffeeNewspaper_CLI
{
    public class BackCommand : BaseCommand
    {
        public BackCommand(BaseState state) : base(state)
        {
            Name = "back";
        }

        public override async Task<BaseState> Excute(ArgumentParser input)
        {
            await Task.Delay(1);
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