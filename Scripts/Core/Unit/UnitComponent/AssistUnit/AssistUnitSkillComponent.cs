using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitSkillComponent : UnitSkillComponent
    {
        public AssistUnitSkillComponent(Unit owner) : base(owner)
        {

        }

        protected override void PlayEffectSound(ResourceEffect effect)
        {
            if (string.IsNullOrEmpty(effect.sound))
            {
                return;
            }

            SoundManager.Instance.PlaySfx(effect.sound, 0.5f);
        }
    }
}