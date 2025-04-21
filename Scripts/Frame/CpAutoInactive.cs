using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CpAutoInactive : MonoBehaviour
{
    [SerializeField] float actTime = 0f;

    private float delta = 0f;
    private Action onClose = null;

    public void SetActTime(float time)
    {
        actTime = time;
    }

    public void SetOnCloseCall(Action onClose)
    {
        this.onClose = onClose;
    }

    public void OnEnable()
    {
        delta = 0f;
    }

    public bool IsActive()
    {
        return gameObject.activeInHierarchy && delta < actTime;
    }

    public void FixedUpdate()
    {
        delta += Time.fixedDeltaTime;

        if (delta >= actTime)
        {
            gameObject.SetActive(false);
            if (onClose != null)
            {
                onClose();
            }
        }
    }
}
