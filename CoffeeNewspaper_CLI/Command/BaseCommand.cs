using System;

namespace CoffeeNewspaper_CLI
{
    public class BaseCommand
    {
        protected BaseState State { get; }

        public BaseCommand(BaseState state)
        {
            State = state;
        }

        protected bool Equals(BaseCommand other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseCommand) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public string Name { get;protected set; }

        public virtual BaseState Excute(ArgumentParser input)
        {
            throw new NotImplementedException();
        }

    }
}