using UnitComponent;

public abstract class Unit : SimplePoolItem
{
    public readonly UnitCoreComponent core = null;

    protected Unit()
    {
        core = CreateCoreComponent();
    }

    protected virtual UnitCoreComponent CreateCoreComponent()
    {
        return UnitCoreComponent.Of(this);
    }

    public override void Initialize()
    {
        core.Initialize();
    }

    public override void DoReset()
    {
        core.DoReset();
    }

    public void UpdateDt(float dt)
    {
        core.UpdateDt(dt);

#if UNITY_EDITOR
        DebugStateView();
#endif
    }

    public override void OnDisable()
    {
        core.OnDisable();
        base.OnDisable();
    }

#if UNITY_EDITOR
    private static System.Text.StringBuilder sbDebugState = new System.Text.StringBuilder();
    private void DebugStateView()
    {
        if (!UnitRule.IsValid(this))
        {
            return;
        }

        sbDebugState.Clear();
        sbDebugState.Append($"uid:[{core.profile.tunit.uid}]\n");
        sbDebugState.Append($"unit id:[{core.profile.tunit.id}]\n");
        sbDebugState.Append($"level:[{core.profile.tunit.GetLevel()}]\n");
        sbDebugState.Append($"hp:[{core.health.hp}/{core.health.maxHp}]\n");

        sbDebugState.Append("\n======[BASE STAT]======\n");
        sbDebugState.Append(core.stat.DebugStatString());

        sbDebugState.Append("\n=======[ABILITY]=======\n");
        sbDebugState.Append(core.stat.DebugAbilityString());

        sbDebugState.Append("\n=======[PENALTY]=======\n");
        sbDebugState.Append(core.stat.DebugPenaltyString());

        sbDebugState.Append("\n=======[GET SKILL]=======\n");
        sbDebugState.Append(core.skill.DebugString());

        sbDebugState.Append("\n=======[GET BUFF]=======\n");
        sbDebugState.Append(core.buff.DebugString());

        core.transform.DebugStateView(sbDebugState.ToString());
    }
#endif

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}

