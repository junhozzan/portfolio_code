using behaviorTree;
using UnityEngine;
using System;
using ModeComponent;

public partial class DamageMode : Mode
{
    public readonly new DamageModeCoreComponent core = null;

    public static DamageMode Of(ResourceMode resMode)
    {
        return new DamageMode(resMode);
    }

    private DamageMode(ResourceMode resMode) : base(resMode)
    {
        this.core = base.core as DamageModeCoreComponent;
    }

    protected override ModeCoreComponent CreateCoreComponent()
    {
        return DamageModeCoreComponent.Of(this);
    }
}
