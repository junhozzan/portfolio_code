using behaviorTree;

public class BehaviorTree : Node
{
    public readonly Node root = null;

    public BehaviorTree(Node root)
    {
        this.root = root;

#if UNITY_EDITOR
        this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
    }

    protected override int Update(float dt)
    {
#if UNITY_EDITOR
        ResetDebugOptions(this);
#endif
        if (root != null)
        {
            root.Tick(dt);
        }

        return Status.SUCCESS;
    }

#if UNITY_EDITOR
    private static void ResetDebugOptions(Node node)
    {
        if (node.debugOption.children != null)
        {
            foreach (var child in node.debugOption.children)
            {
                ResetDebugOptions(child);
            }
        }

        node.debugOption.ResetStatus();
    }
#endif
}
