using UnityEngine;
using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitAIComponent : UnitBaseComponent
    {
        private readonly Dictionary<int, BehaviorTree> behaviorTreePool = new Dictionary<int, BehaviorTree>();
        private BehaviorTree behaviorTree = null;

#if UNITY_EDITOR
        public BehaviorTree _behaviorTree => behaviorTree;
#endif

        public UnitAIComponent(Unit owner) : base(owner)
        {

        }

        public override void DoReset()
        {
            base.DoReset();
            behaviorTree = null;
        }

        public void Refresh()
        {
            var resUnit = owner.core.profile.tunit.resUnit;

            if (!behaviorTreePool.TryGetValue(resUnit.id, out var v))
            {
                var aiScript = Util.GetNewInstance(resUnit.aiScript, owner) as AIScript;
                if (aiScript == null)
                {
                    if (_DEBUG)
                    {
                        Debug.Log(S.Red($"## ai script is null : {resUnit.aiScript}"));
                    }

                    return;
                }

                behaviorTreePool.Add(resUnit.id, v = new BehaviorTree(aiScript));
            }

            behaviorTree = v;
            behaviorTree.Reset();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            if (behaviorTree == null)
            {
                return;
            }

            behaviorTree.Tick(dt);
        }
    }
}