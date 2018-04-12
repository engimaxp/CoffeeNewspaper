using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CN_Model;
using Console = Colorful.Console;

namespace CoffeeNewspaper_CLI
{
    public class TagCommand : BaseCommand
    {
        public TagCommand(BaseState state) : base(state)
        {
            Name = "tag";
        }

        public override BaseState Excute(ArgumentParser input)
        {
            if (State != null && State.StateObj is CNTask task)
            {
                if (task.Tags == null) task.Tags = new List<string>();
                var addTags = input.Get<string>("add");
                if (addTags != null && addTags.Length > 0)
                {
                    var totalTags = addTags.Union(task.Tags);
                    task.Tags = totalTags.ToList();
                }
                var removeTags = input.Get<string>("remove");
                if (removeTags != null && removeTags.Length > 0)
                {
                    var totalTags = task.Tags.Except(removeTags).ToList();
                    task.Tags = totalTags.Any()? totalTags:null;
                }
                State.Refresh();
            }
            else if (State != null && State.StateObj is CNMemo memo)
            {
                if (memo.Tags == null) memo.Tags = new List<string>();
                var addTags = input.Get<string>("add");
                if (addTags != null && addTags.Length > 0)
                {
                    var totalTags = addTags.Union(memo.Tags);
                    memo.Tags = totalTags.ToList();
                }
                var removeTags = input.Get<string>("remove");
                if (removeTags != null && removeTags.Length > 0)
                {
                    var totalTags = memo.Tags.Except(removeTags).ToList();
                    memo.Tags = totalTags.Any() ? totalTags : null;
                }
                State.Refresh();
            }
            else
            {
                Console.WriteLine("wrong parameter", Color.Red);
            }

            Console.WriteLine();
            return State;
        }
        
    }
}