using System;
using UnityEngine;

namespace ModeComponent
{
    public class StageModeProfileComponent : ModeProfileComponent
    {
        public virtual ResourceMode resTargetMode => mode.core.profile.resMode;

        public StageModeProfileComponent(StageMode mode) : base(mode)
        {

        }

        public override long GetModeLevel()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(resTargetMode.id);
            return ResourceManager.Instance.level.GetExpToLevel(tmode.GetExp());
        }

        public override long GetStage()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(resTargetMode.id);
            return tmode.GetScore();
        }

        public override float GetExpRate()
        {
            var tmode = MyPlayer.Instance.core.mode.GetMode(resTargetMode.id);
            var level = GetModeLevel();
            var prev = ResourceManager.Instance.level.GetLevel((int)Math.Max(1, level - 1));
            var curr = ResourceManager.Instance.level.GetLevel((int)level);

            long max;
            long value;
            if (prev != curr)
            {
                max = curr.exclude - prev.exclude;
                value = tmode.GetExp() - prev.exclude;
            }
            else
            {
                max = curr.exclude;
                value = tmode.GetExp();
            }

            return (float)value / max;
        }

        public long GetAddExp()
        {
            //var stage = GetStage();
            //return 1 + (long)Math.Log(stage, 2) + stage;
            return 0;
        }

        public long GetAddGold()
        {
            var stage = GetStage();
            return (long)Mathf.Pow(stage, GameData.DEFAULT.STAGE_ADD_GOLD_VALUE) + 300;
        }
    }
}