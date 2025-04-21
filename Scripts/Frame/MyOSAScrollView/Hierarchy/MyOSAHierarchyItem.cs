using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MyOSAHierarchyItem : MonoBehaviour
{
    protected MyOSAHierarchy.ListVH vh = null;
    protected Action<MyOSAHierarchy.ListVH, Action<bool>> onToggleDirectory { get; set; } = null;

    public abstract void DoReset();
    public abstract void UpdateViews(MyOSAHierarchy.IOsaItem iOsaItem);
    public abstract float GetSize();
    public abstract void Refresh();

    public void SetToggleDirectory(MyOSAHierarchy.ListVH vh, Action<MyOSAHierarchy.ListVH, Action<bool>> onToggleDirectory)
    {
        this.vh = vh;
        this.onToggleDirectory = onToggleDirectory;
    }

    public void ToggleDirectory()
    {
        if (onToggleDirectory == null)
        {
            return;
        }

        onToggleDirectory(vh, OnExpand);
    }

    protected virtual void OnExpand(bool expand)
    {
        // override
    }
}
