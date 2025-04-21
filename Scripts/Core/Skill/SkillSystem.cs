using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Skill
{
    public class SkillSystem
    {
        private ReadOnlyDictionary<SkillScriptType, SimplePool<SkillBase>> factory =
            new Dictionary<SkillScriptType, SimplePool<SkillBase>>()
            {
                [SkillScriptType.ATTACK] = SimplePool<SkillBase>.Of(SkillAttack.Of, 32),
                [SkillScriptType.PROJECTILE] = SimplePool<SkillBase>.Of(SkillProjectile.Of, 32),
                [SkillScriptType.RANGE] = SimplePool<SkillBase>.Of(SkillRange.Of, 32),
                [SkillScriptType.TELEPORT] = SimplePool<SkillBase>.Of(SkillTeleport.Of, 32),
                [SkillScriptType.JUMP] = SimplePool<SkillBase>.Of(SkillJump.Of, 32),
                //[SkillScriptType.PARABOLA] = SimplePool<SkillBase>.Of(SkillParabola.Of, 32),
                //[SkillScriptType.BUFF] = SimplePool<SkillBase>.Of(SkillBuff.Of, 32),
            }
            .ReadOnly();

        public static SkillSystem Of()
        {
            return new SkillSystem();
        }

        public void Clear()
        {
            foreach (var item in factory.Values)
            {
                item.Clear();
            }
        }

        public void UpdateDt(float dt)
        {
            foreach (var pool in factory.Values)
            {
                var list = pool._list;
                for (int i = 0, cnt = list.Count; i < cnt; ++i)
                {
                    var item = list[i];
                    if (!item.IsUsed())
                    {
                        continue;
                    }

                    item.UpdateDt(dt);
                }
            }
        }

        public SkillBase GetSkill(SkillInfo skillInfo)
        {
            return factory.TryGetValue(skillInfo.resFire.script.type, out var pool) ? pool.Pop() : null;
        }

#if USE_DEBUG
        protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
    }
}