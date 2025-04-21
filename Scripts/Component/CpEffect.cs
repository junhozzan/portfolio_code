using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpEffect : CpObject
{
    [SerializeField] ParticleSystem[] particles = null;
    [SerializeField] bool canFlip = false;
    [SerializeField] bool autoFlip = false;
    [SerializeField] string sound = string.Empty;

    private ParticleSystemRenderer[] particleRenderers = null;

    protected override void Init()
    {
        base.Init();

        particleRenderers = new ParticleSystemRenderer[particles.Length];
        for (int i = 0, cnt = particleRenderers.Length; i < cnt; ++i)
        {
            particleRenderers[i] = particles[i].GetComponent<ParticleSystemRenderer>();
        }
    }

    public override void SetLayer(string strLayer, int order = 0)
    {
        base.SetLayer(strLayer, order);

        for (int i = 0, len = particleRenderers.Length; i < len; ++i)
        {
            particleRenderers[i].sortingLayerName = strLayer;
            particleRenderers[i].sortingOrder = order;
        }
    }

    public override void SetLayerOrder(int order)
    {
        base.SetLayerOrder(order);
        for (int i = 0, len = particleRenderers.Length; i < len; ++i)
        {
            particleRenderers[i].sortingOrder = order;
        }
    }

    public override void SetFlip(bool flip)
    {
        if (!canFlip)
        {
            return;
        }

        base.SetFlip(flip);
    }

    private void OnEnable()
    {
        AutoFlip();
        PlaySound();
    }

    private void PlaySound()
    {
        if (string.IsNullOrEmpty(sound))
        {
            return;
        }

        SoundManager.Instance.PlaySfx(sound);
    }

    private void AutoFlip()
    {
        if (!autoFlip)
        {
            return;
        }

        SetFlip(Random.Range(-1f, 1f) >= 0f);
    }
}
