using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyOSAGridItem : MonoBehaviour
{
    public abstract void DoReset();
    public abstract void UpdateViews(MyOSAGrid.IOsaItem tOsaItem);
    public abstract void Refresh();
    public abstract bool IsEmpty();
}
