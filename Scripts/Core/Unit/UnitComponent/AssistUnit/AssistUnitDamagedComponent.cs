using UnityEngine;

namespace UnitComponent
{
    public class AssistUnitDamagedComponent : UnitDamagedComponent
    {
        public AssistUnitDamagedComponent(Unit owner) : base(owner)
        {

        }

        protected override (Color, Color) GetDamagedColor(bool isCritical)
        {
            return (GameData.COLOR.DAMAGE_HIT_TOP, GameData.COLOR.DAMAGE_HIT_BOTTOM);
        }

        protected override (Color, Color) GetHolyDamagedColor()
        {
            return (GameData.COLOR.DAMAGE_HIT_TOP, GameData.COLOR.DAMAGE_HIT_BOTTOM);
        }

        protected override (Color, Color) GetDarkDamagedColor()
        {
            return (GameData.COLOR.DAMAGE_HIT_TOP, GameData.COLOR.DAMAGE_HIT_BOTTOM);
        }

        protected override (Color, Color) GetZeroColor()
        {
            return (GameData.COLOR.DAMAGE_HIT_TOP, GameData.COLOR.DAMAGE_HIT_BOTTOM);
        }
    }
}