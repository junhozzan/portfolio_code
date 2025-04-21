using UnityEngine;
using System.Collections.Generic;

public class CpUnit : CpObject
{
    [SerializeField] SpumCustom spum = null;
    [SerializeField] Animator anim = null;

    private CpUnitStateView stateView = null;

    protected override void Init()
    {
        base.Init();
        spum?.Init();
    }

    protected override List<GameObject> GetIgnoreRenderers()
    {
        var custom = new List<GameObject>();
        custom.AddRange(base.GetIgnoreRenderers());
        
        var shadow = spum.GetShadow();
        if (shadow != null)
        {
            custom.Add(shadow);
        }

        return custom;
    }

    public override void DoReset()
    {
        base.DoReset();
        spum?.DoReset();

        SetAlpha(1f);
    }

    public override void SetScale(float fScale)
    {
        base.SetScale(fScale);

        ForceUpdateColliderVoluem();
    }

    public void SetSkin(ResourceSPUM.PartsType type, ResourceSPUM resSPUM)
    {
        spum?.SetSkinType(type, resSPUM);
    }

    public virtual void Handle_StopMove()
    {
        UpdateCollider();
    }

    public IReadOnlyDictionary<SpriteRenderer,SpumCustom.RendererInfo> GetSpumRenderers()
    {
        return spum.GetAllRenderer();
    }

    public void UpdateStateView(string str)
    {
        if (stateView == null)
        {
            stateView = gameObject.AddComponent<CpUnitStateView>();
        }

        stateView.SetString(str);
    }

    public void SetAnimation(UnitAni ani)
    {
        anim.SetInteger("State", Util.EnumToInt(ani));
    }

    public void SetImmediateAnimation(UnitAni ani)
    {
        anim.Play(ani.ToString(), 0, 0f);
    }

    public void SetAnimationSpeed(float speed)
    {
        anim.speed = speed;
    }

    public bool IsAttackMotion()
    {
        var info = anim.GetCurrentAnimatorStateInfo(0);
        // 좀더 모션이 빨리 스킵되도록 설정
        return info.IsName(UnitAni.ATTACK.ToString()) && info.normalizedTime < 0.95f;
    }
}
