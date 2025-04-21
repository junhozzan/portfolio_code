using UnityEngine;

namespace UnitComponent
{
    public class UnitDamagedComponent : UnitBaseComponent
    {
        public UnitDamagedComponent(Unit owner) : base(owner)
        {

        }

        public void TakeDamage(bool firstHit, long attackerUID, long damage, long holyDamage, long darkDamage)
        {
            TakeDamage(firstHit, attackerUID, damage, holyDamage, darkDamage, false, null);
        }

        public virtual void TakeDamage(bool firstHit, long attackerUID, long damage, long holyDamage, long darkDamage, bool isCritical, ResourceSkillAttack resAttack)
        {
            if (!UnitRule.IsAlive(owner))
            {
                return;
            }

            var sumDamage = damage + holyDamage + darkDamage;
            var prevHp = owner.core.health.hp;
            var newHp = System.Math.Max(prevHp - sumDamage, 0);
            var realDamage = prevHp - newHp;

            owner.core.health.SetHP(newHp);

            CallEvent(realDamage, firstHit, newHp, prevHp);

            var from = UnitManager.Instance.GetUnitByUID(attackerUID);
            if (UnitRule.IsValid(from))
            {
                from.core.damage.Attack(resAttack, owner, realDamage);
            }

            owner.core.buff.HandleAttacked(resAttack, from, realDamage);
            OnAttackedEffect(resAttack, sumDamage, damage, holyDamage, darkDamage, isCritical);

            if (newHp <= 0f)
            {
                owner.core.dead.CheckDead(from);
            }
        }

        protected virtual void CallEvent(long damaged, bool firstHit, long newHp, long prevHp)
        {

        }

        protected void OnAttackedEffect(ResourceSkillAttack resAttack, float sumDamage, float damage, float holyDamage, float darkDamage, bool isCritical)
        {
            var fontPosition = owner.core.transform.GetDamageFontPosition();

            PlayAttackedEffect(resAttack, owner.core.transform);
            PlayAttackedSound(resAttack);

            if (sumDamage <= 0f)
            {
                PopZeroFont(owner, fontPosition, GetZeroColor());
                return;
            }

            PopFont(owner, fontPosition, damage, GetDamagedColor(isCritical));
            PopFont(owner, fontPosition, holyDamage, GetHolyDamagedColor());
            PopFont(owner, fontPosition, darkDamage, GetDarkDamagedColor());

        }

        protected virtual CpUI_DamageFont PopFont(Unit target, Vector3 pos, float damage, (Color, Color) colors)
        {
            if (damage <= 0f)
            {
                return null;
            }

            var font = CpUI_DamageFont.ShowFont(target.core.profile.tunit.uid, pos, damage.ToString("#,###"));
            font.SetTextColor(colors);
            font.UpPosition();

            return font;
        }

        protected virtual CpUI_DamageFont PopZeroFont(Unit target, Vector3 pos, (Color, Color) colors)
        {
            var font = CpUI_DamageFont.ShowFont(target.core.profile.tunit.uid, pos, "0");
            font.SetTextColor(colors);
            font.UpPosition();

            return font;
        }

        private static void PlayAttackedEffect(ResourceSkillAttack resAttack, IEffectPointer effectPointer)
        {
            if (resAttack == null || string.IsNullOrEmpty(resAttack.effect.prefab))
            {
                return;
            }

            if (effectPointer == null)
            {
                return;
            }

            var point = effectPointer.GetPoint(resAttack.effect.pointType);
            if (!point.HasValue)
            {
                return;
            }

            var obj = ObjectManager.Instance.Pop<CpEffect>(resAttack.effect.prefab);
            obj.SetPosition(point.Value);
            obj.SetLayer(resAttack.effect.layer);
            if (!resAttack.effect.IsInfinity())
            {
                obj.DelayInactive(resAttack.effect.time);
            }
        }

        private static void PlayAttackedSound(ResourceSkillAttack resAttack)
        {
            if (resAttack == null || string.IsNullOrEmpty(resAttack.effect.sound))
            {
                return;
            }

            SoundManager.Instance.PlaySfx(resAttack.effect.sound);
        }

        protected virtual (Color, Color) GetDamagedColor(bool isCritical)
        {
            if (isCritical)
            {
                return (GameData.COLOR.DAMAGE_CRITICAL_TOP, GameData.COLOR.DAMAGE_CRITICAL_BOTTOM);
            }

            return (GameData.COLOR.DAMAGE_NORMAL_TOP, GameData.COLOR.DAMAGE_NORMAL_BOTTOM);
        }

        protected virtual (Color, Color) GetHolyDamagedColor()
        {
            return (GameData.COLOR.DAMAGE_HOLY_TOP, GameData.COLOR.DAMAGE_HOLY_BOTTOM);
        }

        protected virtual (Color, Color) GetDarkDamagedColor()
        {
            return (GameData.COLOR.DAMAGE_DARK_TOP, GameData.COLOR.DAMAGE_DARK_BOTTOM);
        }

        protected virtual (Color, Color) GetZeroColor()
        {
            return (GameData.COLOR.MISS_TOP, GameData.COLOR.MISS_BOTTOM);
        }
    }
}