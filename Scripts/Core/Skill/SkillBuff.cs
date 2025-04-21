
/// <summary>
/// not used
/// </summary>
public class SkillBuff : SkillBase
{
    private ResourceSkillBuff res = null;

    public static SkillBuff Of()
    {
        return new SkillBuff();
    }

    public override void DoReset()
    {
        base.DoReset();
        res = null;
    }

    public override bool Play(SkillInfo skillInfo)
    {
        base.Play(skillInfo);

        var target = skillInfo._target;
        if (!UnitRule.IsValid(target))
        {
            OnDisable();
            return false;
        }

        res = skillInfo.resFire.script as ResourceSkillBuff;
        DoBuff(target);

        return true;
    }

    private void DoBuff(Unit target)
    {
        //target.core.buff.add.AddBuffsBySkill(skillInfo._from, res.buffIDs, skillInfo.resSkill.id);

        //var targetPos = target.core.transform.GetCenterPosition();
        //PlayTail(target, targetPos);
        //Finish();
    }
}
