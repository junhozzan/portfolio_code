using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitComponent;

public class EnemyUnit : Unit
{
    public readonly new EnemyUnitCoreComponent core = null;

    public static EnemyUnit Of()
    {
        return new EnemyUnit();
    }

    protected EnemyUnit() : base()
    {
        this.core = base.core as EnemyUnitCoreComponent;
    }

    protected override UnitCoreComponent CreateCoreComponent()
    {
        return EnemyUnitCoreComponent.Of(this);
    }
}
