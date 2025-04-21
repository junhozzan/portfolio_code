using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Text Gradient")]
public class UITextGradient : BaseMeshEffect
{
    [SerializeField] AnimationCurve m_colorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] Color m_colorTop = Color.white;
    [SerializeField] Color m_colorBottom = Color.white;
	[Range(-180f, 180f)]
    [SerializeField] float m_angle = 0f;

    public void Set(Color top, Color bottom)
    {
        enabled = false;
        m_colorTop = top;
        m_colorBottom = bottom;
        enabled = true;
    }
    public override void ModifyMesh(VertexHelper vh)
    {
		if(enabled)
		{
			Rect rect = graphic.rectTransform.rect;
			Vector2 dir = UIGradientUtils.RotationDir(m_angle);
			UIGradientUtils.Matrix2x3 localPositionMatrix = UIGradientUtils.LocalPositionMatrix(new Rect(0f, 0f, 1f, 1f), dir);

			UIVertex vertex = default(UIVertex);
			for (int i = 0; i < vh.currentVertCount; i++) {

				vh.PopulateUIVertex (ref vertex, i);
				Vector2 position = UIGradientUtils.VerticePositions[i % 4];
				Vector2 localPosition = localPositionMatrix * position;
				vertex.color *= Color.Lerp(m_colorBottom, m_colorTop, m_colorCurve.Evaluate(localPosition.y));
				vh.SetUIVertex (vertex, i);
			}
		}
    }
}
