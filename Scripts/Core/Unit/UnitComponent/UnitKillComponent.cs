using System.Collections.Generic;
using UnityEngine;

namespace UnitComponent
{
    public class UnitKillComponent : UnitBaseComponent
    {
        private readonly SimplePool<KillInfo> killedInfoPool = SimplePool<KillInfo>.Of(KillInfo.Of, 32);
        private readonly Stack<KillInfo> killedInfos = new Stack<KillInfo>();

        public UnitKillComponent(Unit owner) : base(owner)
        {

        }

        public void ClearInfos()
        {
            killedInfos.Clear();
            killedInfoPool.Clear();
        }

        public virtual void Add(Unit unit)
        {
            if (!UnitRule.IsValid(unit))
            {
                return;
            }

            owner.core.buff.HandleKill(unit);

            var info = killedInfoPool.Pop();
            info.Set(unit);

            killedInfos.Push(info);
        }

        public KillInfo Pop()
        {
            if (killedInfos.Count == 0)
            {
                return null;
            }

            return killedInfos.Pop();
        }

        public class KillInfo : SimplePoolItem
        {
            public Vector2 position { get; private set; } = Vector2.zero;
            public float scale { get; private set; } = 1f;
            public ResourceSPUM resSpum { get; private set; } = null;
            public readonly List<ResourceSPUM> skins = new List<ResourceSPUM>();

            public static KillInfo Of()
            {
                return new KillInfo();
            }

            public void Set(Unit unit)
            {
                position = unit.core.transform.GetPosition();
                scale = unit.core.profile.tunit.resUnit.scale;
                resSpum = ResourceManager.Instance.spum.GetSPUM(unit.core.profile.tunit.resUnit.spumID);

                skins.Clear();
                skins.AddRange(unit.core.skin.GetCustomSkins());
            }
        }
    }
}