using System.Collections.Generic;

namespace MyPlayerComponent
{
    public class MyPlayerMergeComponent : MyPlayerBaseComponent
    {
        private readonly Dictionary<int, TMerge> merges = new Dictionary<int, TMerge>();

        public MyPlayerMergeComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_MERGE_DATAS, Handle_GET_MERGE_DATAS)
                .Add(GameEventType.UPDATE_MERGE, Handle_UPDATE_MERGE)
                ;
        }

        private void Handle_GET_MERGE_DATAS(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_MERGE_DATAS>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateMerges(tArg.tmergs);
        }

        private void Handle_UPDATE_MERGE(object[] args)
        {
            var tmergs = GameEvent.GetSafe<ICollection<TMerge>>(args, 0);
            if (tmergs == null || tmergs.Count == 0)
            {
                return;
            }

            UpdateMerges(tmergs);
        }

        private void UpdateMerges(IEnumerable<TMerge> merges)
        {
            foreach (var merge in merges)
            {
                UpdateMerge(merge);
            }
        }

        private void UpdateMerge(TMerge merge)
        {
            if (!merges.TryGetValue(merge.id, out var v))
            {
                merges.Add(merge.id, null);
            }

            if (v != null)
            {
                v.OnDisable();
            }

            merges[merge.id] = merge;
        }

        public ICollection<TMerge> GetMerges()
        {
            return merges.Values;
        }
    }
}