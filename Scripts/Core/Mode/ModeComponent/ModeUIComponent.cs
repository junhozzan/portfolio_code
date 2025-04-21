using UnityEngine;
using UIMode;
using System.Collections.Generic;

namespace ModeComponent
{
    /// <summary>
    /// 가장 마지막에 호출 되어야 한다.
    /// </summary>
    public class ModeUIComponent : ModeBaseComponent
    {
        public readonly ModeUICardComponent card = null;

        private float dpsTextTimer = 0f;

        protected CpUI_Mode ui 
        { 
            get 
            { 
                return CpUI_Mode.Instance; 
            } 
        }

        public ModeUIComponent(Mode mode) : base(mode)
        {
            card = AddComponent<ModeUICardComponent>(mode);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.MODE_START, Handle_MODE_START)
                .Add(GameEventType.MODE_PLAY, Handle_MODE_PLAY)
                .Add(GameEventType.REFRESH_SCORE, Handle_REFRESH_SCORE)
                .Add(GameEventType.REFRESH_EXP, Handle_REFRESH_EXP)
                .Add(GameEventType.CHANGE_MYUNIT_HP, Handle_CHANGE_MYUNIT_HP)
                .Add(GameEventType.CHANGE_MYUNIT_MP, Handle_CHANGE_MYUNIT_MP)
                .Add(GameEventType.ENEMY_ATTACKED, Handle_ENEMY_ATTACKED)
                .Add(GameEventType.GET_MODE_REWARD, Handle_GET_MODE_REWARD)
                .Add(GameEventType.UPDATE_CARD, Handle_UPDATE_CARD)
                ;
        }

        public override void DoReset()
        {
            base.DoReset();
            dpsTextTimer = 0f;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            UIBaseTop.CpUI_BaseTop.Instance.On();
            UIMenu_Bottom.CpUI_MenuBottom.Instance.On();

            // Close순서 때문에 호출 순서 중요.
            ui.On();
            UIMenu.CpUI_Menu.Instance.On();
        }

        public override void UpdateDt(float dt)
        {
            base.UpdateDt(dt);
            UpdateDpsText(dt);
        }

        private void UpdateDpsText(float dt)
        {
            dpsTextTimer += dt;
            if (dpsTextTimer < 1f)
            {
                return;
            }

            dpsTextTimer = 0f;

            var myUnit = mode.core.ally.myUnit;
            if (myUnit != null)
            {
                ui.SetDpsText(myUnit.core.damage.GetDpsString());
            }
        }

        protected virtual void Handle_UPDATE_CARD(object[] args)
        {
            OpenCardSelectPopup();
        }

        private void Handle_MODE_START(object[] args)
        {
            ui.SetDefault();
            ui.SetScoreText(GetScoreText());
            ui.SetBestScoreText(GetBestScoreValueText());

            //var expRate = mode.core.profile.GetExpRate();
            //ui.SetLevel(mode.core.profile.GetModeLevel(), expRate);
            //ui.SetExpGage(expRate);

            OpenCardSelectPopup();
        }

        private void Handle_MODE_PLAY(object[] args)
        {
            var myUnit = mode.core.ally.myUnit;
            if (myUnit != null)
            {
                ui.SetDpsText(myUnit.core.damage.GetDpsString());
                ui.SetMainUnitHp(myUnit.core.health.GetHpRate());
                ui.SetMainUnitMp(myUnit.core.mana.GetMpRate());
            }
        }

        private void Handle_REFRESH_SCORE(object[] args)
        {
            ui.SetScoreText(GetScoreText());
            ui.SetBestScoreText(GetBestScoreValueText());
        }

        protected void Handle_REFRESH_EXP(object[] args)
        {
            //var expRate = mode.core.profile.GetExpRate();
            //ui.SetLevel(mode.core.profile.GetModeLevel(), expRate);
            //ui.SetExpGage(expRate);
        }

        private void OpenCardSelectPopup()
        {
            if (mode.resMode.modeCardIDs.Count == 0
                || mode.core.card.ReceivableCardCount() <= 0)
            {
                return;
            }

            if (card.IsOpenedCardSelect())
            {
                return;
            }

            card.OpenCardSelect();
        }

        private void Handle_ENEMY_ATTACKED(object[] args)
        {
            var tArg = GameEvent.GetSafeS<EnemyAttacked>(args, 0);
            if (tArg == null)
            {
                return;
            }

            var unit = UnitManager.Instance.GetUnitByUID(tArg.Value.unitUID);
            if (!UnitRule.IsValid(unit))
            {
                return;
            }

            var maxDamage = mode.core.ally.GetMaxDamage();
            var a = 15 * System.Math.Max(1L, maxDamage);
            var maxHp = unit.core.health.maxHp;
            var count = (long)unit.core.profile.tunit.resUnit.hpBarCount;
            if (unit.core.profile.tunit.resUnit.isDynamicHpBarCount && maxHp > a)
            {
                count = Mathf.CeilToInt(maxHp / (float)a);
                maxHp = count * a;
            }

            ui.SetMonsterHpSlider(
                tArg.Value.firstHit,
                tArg.Value.unitUID,
                maxHp,
                tArg.Value.newHp,
                tArg.Value.prevHp,
                maxHp / Mathf.Max(1f, count),
                unit.core.profile.tunit.resUnit.hpBarSize);
        }

        private void Handle_CHANGE_MYUNIT_HP(object[] args)
        {
            var myUnit = mode.core.ally.myUnit;
            if (!UnitRule.IsValid(myUnit))
            {
                return;
            }

            ui.SetMainUnitHp(myUnit.core.health.GetHpRate());
        }

        private void Handle_CHANGE_MYUNIT_MP(object[] args)
        {
            var myUnit = mode.core.ally.myUnit;
            if (!UnitRule.IsValid(myUnit))
            {
                return;
            }

            ui.SetMainUnitMp(myUnit.core.mana.GetMpRate());
        }

        private void Handle_GET_MODE_REWARD(object[] args)
        {
            var getInfos = GameEvent.GetSafe<IList<GetInfo>>(args, 0);
            var addGold = GameEvent.GetSafeS<long>(args, 1);

            ui.ShowGetItems(addGold.HasValue ? addGold.Value : 0, getInfos);
        }

        public string GetScoreValueText()
        {
            return ScoreValueToText(mode.core.score.GetScore());
        }

        public string GetBestScoreValueText()
        {
            return ScoreValueToText(mode.core.score.GetBestScore());
        }

        public virtual string GetScoreText()
        {
            return GetScoreValueText();
        }

        public virtual string ScoreValueToText(long score)
        {
            return Util.ToComma(score);
        }
    }
}