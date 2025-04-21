using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace behaviorTree
{
    public class Node
    {
#if UNITY_EDITOR
        public DebugOption debugOption = null;
#endif

        public virtual void Reset()
        {

        }

        protected virtual int Update(float dt)
        {
            return Status.NONE;
        }

        public int Tick(float dt)
        {
            var status = Update(dt);
#if UNITY_EDITOR
            debugOption.status = status;
#endif
            return status;
        }

        public class Status
        {
            public const int NONE = 0;
            public const int SUCCESS = 1;
            public const int FAILURE = 2;
        }
    }

    public class Composite : Node
    {
        public readonly Node[] nodes = null;

        public Composite(params Node[] nodes)
        {
            this.nodes = nodes;
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0, cnt = nodes.Length; i < cnt; ++i)
            {
                nodes[i].Reset();
            }
        }
    }


    public class Sequence : Composite
    {
        /// <summary>
        /// 모두 성공이여야 성공.
        /// 실패시 로직 중단.
        /// </summary>
        public Sequence(params Node[] nodes) : base(nodes)
        {
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (nodes != null)
            {
                for (int i = 0, cnt = nodes.Length; i < cnt; ++i)
                {
                    var status = nodes[i].Tick(dt);
                    if (status == Status.FAILURE)
                    {
                        return Status.FAILURE;
                    }
                }
            }

            return Status.SUCCESS;
        }
    }


    public class Selector : Composite
    {
        /// <summary>
        /// 성공이 나오면 성공.
        /// 성공이 나올때까지 로직 실행.
        /// </summary>
        public Selector(params Node[] nodes) : base(nodes) 
        {
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (nodes != null)
            {
                for (int i = 0, cnt = nodes.Length; i < cnt; ++i)
                {
                    var status = nodes[i].Tick(dt);
                    if (status == Status.SUCCESS)
                    {
                        return Status.SUCCESS;
                    }
                }
            }

            return Status.FAILURE;
        }
    }

    public class Decorator : Node
    {
        public readonly Node node = null;

        public Decorator(Node node)
        {
            this.node = node;
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        public override void Reset()
        {
            base.Reset();
            if (node != null)
            {
                node.Reset();
            }
        }
    }

    public class Inverter : Decorator
    {
        public Inverter(Node node) : base(node) 
        {
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (node != null)
            {
                var status = node.Tick(dt);
                if (status == Status.SUCCESS)
                {
                    return Status.FAILURE;
                }
                else if (status == Status.FAILURE)
                {
                    return Status.SUCCESS;
                }
            }

            return Status.NONE;
        }
    }

    public class Succeeder : Decorator
    {
        public Succeeder(Node node) : base(node)
        {
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (node != null)
            {
                node.Tick(dt);
            }

            return Status.SUCCESS;
        }
    }

    public class Failer : Decorator
    {
        public Failer(Node node) : base(node)
        {
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: GetType().Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (node != null)
            {
                node.Tick(dt);
            }

            return Status.FAILURE;
        }
    }

    public class AIAction : Node
    {
        private readonly Action<float> onAction = null;

        public AIAction(Action onAction)
        {
            this.onAction = (i) => onAction();
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: onAction.Method.Name);
#endif
        }

        public AIAction(Action<float> onAction)
        {
            this.onAction = onAction;
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: onAction.Method.Name);
#endif
        }

        protected override int Update(float dt)
        {
            if (onAction != null)
            {
                onAction(dt);
            }

            return Status.SUCCESS;
        }
    }

    public class AIResultAction : Node
    {
        private readonly Func<int> onAction = null;

        public AIResultAction(Func<int> onAction, string name)
        {
            this.onAction = onAction;
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name);
#endif
        }

        public AIResultAction(Func<int> onAction)
        {
            this.onAction = onAction;
#if UNITY_EDITOR
            this.debugOption = new DebugOption(this, name: onAction.Method.Name);
#endif
        }

        protected override int Update(float dt)
        {
            var status = Status.NONE;
            if (onAction != null)
            {
                status = onAction();
            }

            return status;
        }
    }
#if UNITY_EDITOR
    public class DebugOption
    {
        public readonly Node node;
        public readonly string name;
        public readonly bool collapsible;

        public int status = Node.Status.NONE;
        public bool collapse = false;

        public IReadOnlyList<Node> children;

        public DebugOption(Node node, string name = "")
        {
            this.node = node;
            this.name = name.Replace("\r", "").Replace("\n", "");

            var childList = new List<Node>();
            if (node is Composite c)
            {
                childList.AddRange(c.nodes);
            }
            else if (node is Decorator d)
            {
                childList.Add(d.node);
            }
            else if (node is BehaviorTree b)
            {
                childList.Add(b.root);
            } 
            else if (node is AIScript ai)
            {
                childList.Add(ai.ai);
            }

            this.children = childList;
            this.collapsible = childList.Count > 0;
        }

        public string DisplayText
        {
            get
            {
                return collapse ? $"(...) {name}" : name;
            }
        }

        public void ToggleCollapse()
        {
            collapse = !collapse;
        }

        public void ResetStatus()
        {
            status = Node.Status.NONE;
        }

        public bool IsActive => status != Node.Status.NONE;
    }
#endif
}