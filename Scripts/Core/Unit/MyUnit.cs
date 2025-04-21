using System.Collections.Generic;
using UnitComponent;

public class MyUnit : Unit
{
    public static MyUnit Instance { get; private set; } = null;

    public readonly new MyUnitCoreComponent core = null;

    public static MyUnit Of()
    {
        return new MyUnit();
    }

    private MyUnit() : base()
    {
        Instance = this;
        this.core = base.core as MyUnitCoreComponent;

        CreateHandler();
    }

    private void CreateHandler()
    {
        GameEvent.Instance.CreateHandler(this, IsUsed)
            .Add(GameEventType.UPDATE_LAB, Handle_UPDATE_LAB)
            .Add(GameEventType.UPDATE_COLLECTION, Handle_UPDATE_COLLECTION)
            .Add(GameEventType.UPDATE_ITEM_ALL, Handle_UPDATE_ITEM_ALL)
            .Add(GameEventType.UPDATE_ITEM_NEW, Handle_UPDATE_ITEM_NEW)
            .Add(GameEventType.EQUIP_ITEM, Handle_EQUIP_ITEM)
            ;
    }

    protected override UnitCoreComponent CreateCoreComponent()
    {
        return MyUnitCoreComponent.Of(this);
    }

    private void Handle_UPDATE_LAB(object[] args)
    {
        core.refresh.NextRefresh();
    }

    private void Handle_UPDATE_ITEM_NEW(object[] args)
    {
        core.refresh.NextRefresh();
    }

    private void Handle_UPDATE_ITEM_ALL(object[] args)
    {
        core.refresh.NextRefresh();
    }

    private void Handle_UPDATE_COLLECTION(object[] args)
    {
        core.refresh.NextRefresh();
    }

    private void Handle_EQUIP_ITEM(object[] args)
    {
        core.skin.RefreshSkin();
        core.refresh.NextRefresh();
    }
}