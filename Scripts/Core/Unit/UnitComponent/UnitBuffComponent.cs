using System.Collections.Generic;

namespace UnitComponent
{
    public class UnitBuffComponent : UnitBaseComponent
    {
        public readonly UnitBuffBuffScriptComponent buffScript = null;
        public readonly UnitBuffStatComponent stat = null;
        public readonly UnitBuffAddComponent add = null;
        public readonly UnitBuffRemoveComponent remove = null;
        public readonly UnitBuffItemBuffComponent itemBuff = null;
        public readonly UnitBuffModeBuffComponent modeBuff = null;
        public readonly UnitBuffLabBuffComponent labBuff = null;

        private readonly SimplePool<UnitBuff> buffPool = SimplePool<UnitBuff>.Of(UnitBuff.Of, 32);
        private readonly Dictionary<int, UnitBuff> buffs = new Dictionary<int, UnitBuff>();

        public UnitBuffComponent(Unit owner) : base(owner)
        {
            buffScript = AddComponent<UnitBuffBuffScriptComponent>(owner);
            stat = AddComponent<UnitBuffStatComponent>(owner);
            add = AddComponent<UnitBuffAddComponent>(owner);
            remove = AddComponent<UnitBuffRemoveComponent>(owner);
            itemBuff = AddComponent<UnitBuffItemBuffComponent>(owner);
            modeBuff = AddComponent<UnitBuffModeBuffComponent>(owner);
            labBuff = AddComponent<UnitBuffLabBuffComponent>(owner);
        }

        public override void DoReset()
        {
            base.DoReset();

            foreach (var buff in buffs.Values)
            {
                buff.OnDisable();
            }

            buffs.Clear();
            buffPool.Clear();
        }

        public override void OnDisable()
        {
            foreach (var buff in buffs.Values)
            {
                buff.OnDisable();
            }

            buffs.Clear();
            buffPool.Clear();

            base.OnDisable();
        }

        public void Refresh()
        {
            modeBuff.Refresh();
            itemBuff.Refresh();
            labBuff.Refresh();

            UpdateRemoveAndNew(false);
        }

        public void HandleDead()
        {
            foreach (var buff in buffs.Values)
            {
                remove.AddRemoveBuffID(buff.tbuff.id);
            }

            UpdateRemoveAndNew(false);
        }

        public virtual UnitBuff CreateUnitBuff(TBuff tbuff)
        {
            if (!buffs.TryGetValue(tbuff.id, out var buff))
            {
                buffs.Add(tbuff.id, buff = buffPool.Pop());
            }

            buff.UpdateTBuff(tbuff);
            buffScript.AddBuffScript(buff);

            return buff;
        }

        public UnitBuff RemoveUnitBuff(int id)
        {
            if (!buffs.TryGetValue(id, out var buff))
            {
                return null;
            }

            buffs.Remove(id);
            buffScript.RemoveBuffScript(buff);

            return buff;
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdateRemoveAndNew(true);
            UpdateBuffs(dt);
        }

        public void UpdateRemoveAndNew(bool isRefresh)
        {
            var checkUpdated = false;

            checkUpdated |= remove.UpdateRemove();
            checkUpdated |= add.UpdateAdd();

            if (isRefresh && checkUpdated)
            {
                owner.core.refresh.NextRefresh();
            }
        }

        private void UpdateBuffs(float dt)
        {
            var nowEpochSecond = Main.Instance.time.nowToEpochSecond();
            foreach (var buff in buffs.Values)
            {
                buff.UpdateDt(dt);

                if (!buff.IsBuffTime(nowEpochSecond))
                {
                    remove.AddRemoveBuffID(buff.tbuff.id);
                }
            }
        }

        public void HandleUseSkill(Unit to)
        {
            foreach (var buff in buffs.Values)
            {
                buff.HandleUseSkill(to);
            }
        }

        public void HandleAttack(ResourceSkillAttack resAttack, Unit to, float damage)
        {
            foreach (var buff in buffs.Values)
            {
                buff.Attack(resAttack, to, damage);
            }
        }

        public void HandleAttacked(ResourceSkillAttack resAttack, Unit from, float damage)
        {
            foreach (var buff in buffs.Values)
            {
                buff.Attacked(resAttack, from, damage);
            }
        }

        public void HandleKill(Unit to)
        {
            foreach (var buff in buffs.Values)
            {
                buff.Kill(to);
            }
        }

        public void HandleKilled(Unit from)
        {
            foreach (var buff in buffs.Values)
            {
                buff.Killed(from);
            }
        }

        public bool HasBuffByFromResSkillID(int resSkillID)
        {
            foreach (var buff in buffs.Values)
            {
                if (buff.tbuff.fromResType != typeof(ResourceSkill))
                {
                    continue;
                }

                if (buff.tbuff.fromResID != resSkillID)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        public UnitBuff GetBuffByID(int id)
        {
            return buffs.TryGetValue(id, out var v) ? v : null;
        }

        public ICollection<int> GetBuffIDs()
        {
            return buffs.Keys;
        }

        public ICollection<UnitBuff> GetBuffs()
        {
            return buffs.Values;
        }

        public float GetAddDamageRatioByBuff(DamageType damageType, Unit to)
        {
            var value = 0f;
            foreach (var buff in buffs.Values)
            {
                value += buff.GetAddDamageRatio(damageType, to);
            }

            return value;
        }

#if UNITY_EDITOR
        private static System.Text.StringBuilder sbDebugState = new System.Text.StringBuilder();
        public string DebugString()
        {
            sbDebugState.Clear();
            foreach (var buff in buffs.Values)
            {
                sbDebugState.Append($"resBuffID:[{buff.tbuff.id}] untilAt:[{buff.tbuff.untilAt}]\n");
            }

            return sbDebugState.ToString();
        }
#endif
    }
}
