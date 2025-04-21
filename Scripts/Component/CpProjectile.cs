using UnityEngine;

public class CpProjectile : CpEffect
{
    [SerializeField] SpriteRenderer spriteRenderer = null;

    public override void DoReset()
    {
        base.DoReset();
        SetRotate2D(Vector2.zero);
    }

    public virtual void Set(Vector3 shotPos)
    {
        SetPosition(shotPos);
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            return;
        }

        this.spriteRenderer.sprite = sprite;
    }

    public void Move(Vector3 pos)
    {
        SetPosition(pos);
    }
}
