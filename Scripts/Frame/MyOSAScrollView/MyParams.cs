using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Com.ForbiddenByte.OSA.Core;
using Com.ForbiddenByte.OSA.CustomParams;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;

[Serializable]
public class MyBaseParamsWithPrefab : BaseParamsWithPrefab
{
    [SerializeField] MyParams myParams = new MyParams();
    public MyParams _myParams
    {
        get
        {
            if (myParams == null)
            {
                myParams = new MyParams();
            }

            return myParams;
        }
    }

    public override void InitIfNeeded(IOSA iAdapter)
    {
        base.InitIfNeeded(iAdapter);
        myParams.Init(this);
    }
}

[Serializable]
public class MyGridParams : GridParams
{
    [SerializeField] MyParams myParams = new MyParams();
    public MyParams _myParams
    {
        get
        {
            if (myParams == null)
            {
                myParams = new MyParams();
            }

            return myParams;
        }
    }

    public override void InitIfNeeded(IOSA iAdapter)
    {
        base.InitIfNeeded(iAdapter);
        myParams.Init(this);
    }
}

[Serializable]
public class MyParams
{
    /// <summary>
    /// true : Content 크기가 초과된 경우만 drag
    /// </summary>
    [SerializeField] bool onlyOverSizeDrag = false;

    /// <summary>
    /// 스크롤바 오프셋
    /// </summary>
    [SerializeField] bool applyBarOffset = false;

    /// <summary>
    /// 자동 리스트 사이즈
    /// </summary>
    [SerializeField] bool autoListSize = true;   
    
    private BaseParams baseParam = null;
    public bool _autoListSize { get { return autoListSize; } }

    public void Init(BaseParams baseParams)
    {
        baseParam = baseParams;
    }

    public void UpdateScrollSize(double viewPortSize, double contentSize)
    {
        double rate = viewPortSize / contentSize;
        bool bOver = rate <= 0.99d;

        if (onlyOverSizeDrag)
        {
            baseParam.DragEnabled = baseParam.ScrollEnabled = bOver;
        }

        if (applyBarOffset)
        {
            Vector2 localPos = baseParam.Content.localPosition;
            localPos.x = bOver ? -3f : 0f;

            baseParam.Content.localPosition = localPos;
        }
    }
}
