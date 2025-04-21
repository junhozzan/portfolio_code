using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MyOSABasicItem : MonoBehaviour
{
    public abstract void DoReset();
    public abstract void UpdateViews(MyOSABasic.IOsaItem tOsaItem);
    public abstract void Refresh();

    public virtual float GetSize() 
    {
        return 0f; 
    }
}
