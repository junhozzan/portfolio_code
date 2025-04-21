using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CustomOutline : BaseMeshEffect
{
    [SerializeField]
    private Color m_EffectColor = new Color(0f, 0f, 0f, 1.0f);

    [SerializeField]
    private float m_EffectDistance = 1.0f;

    [SerializeField]
    private int m_nEffectNumber = 4;

    [SerializeField]
    private bool m_UseGraphicAlpha = true;

    private Color32 m_effectColor32
    {
        get
        {
            return 
                new Color32()
                {
                    r = (byte)(m_EffectColor.r * 255),
                    g = (byte)(m_EffectColor.g * 255),
                    b = (byte)(m_EffectColor.b * 255),
                    a = m_UseGraphicAlpha ? (byte)(m_EffectColor.a * 255) : (byte)255
                };
        }
    }

    private List<UIVertex> m_vVertex = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        m_vVertex.Clear();
        vh.GetUIVertexStream(m_vVertex);

        ModifyVertices(m_vVertex);

        //vh.Clear();
        vh.AddUIVertexTriangleStream(m_vVertex);
    }

    private void ModifyVertices(List<UIVertex> verts)
    {
        Color32 color32 = m_effectColor32;

        int start = 0;
        int end = verts.Count;
        float rad = 0f;
        float v = 2.0f * Mathf.PI / m_nEffectNumber;

        for (int n = 0; n < m_nEffectNumber; ++n)
        {
            rad = v * n;

            ApplyShadow(verts, color32, start, end, m_EffectDistance * Mathf.Cos(rad), m_EffectDistance * Mathf.Sin(rad));

            start = end;
            end = verts.Count;
        }
    }

    private void ApplyShadow(List<UIVertex> verts, Color32 color32, int start, int end, float x, float y)
    {
        UIVertex vt;
        Vector3 v3Pos;

        for (int i = start; i < end; ++i)
        {
            vt = verts[i];
            verts.Add(vt);

            v3Pos = vt.position;
            v3Pos.x += x;
            v3Pos.y += y;

            vt.position = v3Pos;
            vt.color = color32;
            verts[i] = vt;
        }
    }

    public void SetColor(Color color)
    {
        m_EffectColor = color;
        enabled = false;
        enabled = true;
    }
}
