using UnityEngine;

namespace ModeComponent
{
    public class ModeAllyComponent : ModeBaseComponent
    {
        public MyUnit myUnit { get; protected set; } = null;

        public ModeAllyComponent(Mode mode) : base(mode)
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();
            myUnit = mode.core.saved.myUnit;
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                .Add(GameEventType.CHANGE_UNIT_COUNT, Handle_CHANGE_UNIT_COUNT)
                ;
        }

        public void SpawnMyUnit()
        {
            if (myUnit == null)
            {
                var resUnit = ResourceManager.Instance.unit.GetUnit(GameData.UNIT_DATA.ID_HERO);
                myUnit = UnitManager.Instance.Spawn(typeof(MyUnit), resUnit, GetMyUnitLevel()) as MyUnit;
                myUnit.core.transform.CreateObject(Vector3.zero);
                myUnit.core.refresh.RefreshByCreate();
                myUnit.core.fof.SetTeam(Team.ALLY);


                GameEvent.Instance.AddEvent(GameEventType.JOIN_MY_UNIT);
            }

            GameEvent.Instance.AddEvent(GameEventType.CHANGE_UNIT_COUNT);
        }

        private void Handle_MODE_START(object[] args)
        {
            if (myUnit != null)
            {
                myUnit.core.refresh.RefreshByStart();
                myUnit.core.spawn.SetTrailEffect();
            }
        }

        private void Handle_CHANGE_UNIT_COUNT(object[] args)
        {
            if (myUnit != null)
            {
                myUnit.core.target.ResetTargets();
                foreach (var unit in myUnit.core.spawn.units)
                {
                    unit.core.target.ResetTargets();
                }
            }
        }


        public void SetToZero()
        {
            if (myUnit != null)
            {
                myUnit.core.spawn.SetToZero(myUnit.core.transform.GetPosition());
                myUnit.core.spawn.RemoveTrailEffect();
                myUnit.core.transform.SetPosition(Vector3.zero);
            }
        }

        public bool IsMainUnitAlive()
        {
            return UnitRule.IsAlive(myUnit);
        }

        public bool IsLoading()
        {
            if (!UnitRule.IsValid(myUnit))
            {
                return true;
            }

            return false;
        }

        public virtual long GetMyUnitLevel()
        {
            return ResourceManager.Instance.level.GetRiseValue(mode.core.profile.GetModeLevel());
        }

        public long GetMaxDamage()
        {
            if (myUnit == null)
            {
                return 0L;
            }

            return System.Math.Max(myUnit.core.stat.GetLongValue(eAbility.POWER), myUnit.core.stat.GetLongValue(eAbility.POWER_DARK)); 
        }

        public virtual bool IsStartRecovery()
        {
            return true;
        }
    }
}