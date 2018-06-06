using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CN_Core;
using Colorful;

namespace CoffeeNewspaper_CLI
{
    public class TagCommand : BaseCommand
    {
        public TagCommand(BaseState state) : base(state)
        {
            Name = "tag";
        }

        public override async Task<BaseState> Excute(ArgumentParser input)
        {
            await Task.Delay(1);
            if (State != null && State.StateObj is CNTask task)
            {
                var addTags = input.Get<string>("add").Select(x => new CNTag {Title = x}).ToList();
                if (addTags.Any())
                {
                    //TODO:Extract to Service Implement
                    //union to get a non-repeat TagList
                    var totalTags = addTags.Union(task.TaskTaggers.Select(x => x.Tag).ToList(), CNTag.TitleComparer);
                    //select the new tags to be added
                    var toBeAddedTag = totalTags.Where(x => string.IsNullOrEmpty(x.TagId));
                    //add them to task
                    toBeAddedTag.ToList().ForEach(y =>
                        task.TaskTaggers.Add(new CNTaskTagger {Tag = y, TaskId = task.TaskId}));
                }

                var removeTags = input.Get<string>("remove").Select(x => new CNTag {Title = x}).ToList();
                if (removeTags.Any())
                {
                    var toBeRemovedTag = task.TaskTaggers.Select(x => x.Tag).ToList()
                        .Intersect(removeTags, CNTag.TitleComparer).ToList();
                    toBeRemovedTag.ForEach(x =>
                        task.TaskTaggers.Remove(task.TaskTaggers.FirstOrDefault(y => y.TagId == x.TagId)));
                }

                State.Refresh();
            }
            else if (State != null && State.StateObj is CNMemo memo)
            {
                var addTags = input.Get<string>("add").Select(x => new CNTag {Title = x}).ToList();
                if (addTags.Any())
                {
                    //TODO:Extract to Service Implement
                    //union to get a non-repeat TagList
                    var totalTags = addTags.Union(memo.MemoTaggers.Select(x => x.Tag).ToList(), CNTag.TitleComparer);
                    //select the new tags to be added
                    var toBeAddedTag = totalTags.Where(x => string.IsNullOrEmpty(x.TagId));
                    //add them to memo
                    toBeAddedTag.ToList().ForEach(y =>
                        memo.MemoTaggers.Add(new CNMemoTagger {Tag = y, MemoId = memo.MemoId}));
                }

                var removeTags = input.Get<string>("remove").Select(x => new CNTag {Title = x}).ToList();
                if (removeTags.Any())
                {
                    var toBeRemovedTag = memo.MemoTaggers.Select(x => x.Tag).ToList()
                        .Intersect(removeTags, CNTag.TitleComparer).ToList();
                    toBeRemovedTag.ForEach(x =>
                        memo.MemoTaggers.Remove(memo.MemoTaggers.FirstOrDefault(y => y.TagId == x.TagId)));
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