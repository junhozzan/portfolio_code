using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System;
using Com.ForbiddenByte.OSA.Core;

public class MyOSAHierarchy : OSA<MyBaseParamsWithPrefab, MyOSAHierarchy.ListVH>
{
    private readonly ListInfo listInfo = new ListInfo();
    private readonly List<MyOSAHierarchyItem> listObjects = new List<MyOSAHierarchyItem>();
    private Action<MyOSAHierarchyItem> onCreateItem = null;

    protected override void Awake()
    {
        if (IsInitialized)
        {
            return;
        }

        base.Init();

        // 원본 프리펩 숨기기
        _Params.ItemPrefab.gameObject.SetActive(false);
        // 리스트 사이즈가 0일경우 Destroy 방지
        _Params.optimization.KeepItemsPoolOnEmptyList = true;
        _Params.effects.MaxSpeed = 2000;
        _Params._myParams.UpdateScrollSize(GetViewportSize(), GetContentSize());
    }

    protected override void Start() { }

    public void Init(Action<MyOSAHierarchyItem> onCreateItem)
    {
        this.onCreateItem = onCreateItem;
    }

    public void SetList(IOsaItem rootItem)
    {
        listInfo.Set(rootItem);
        ResetItems(listInfo.all.Count, keepVelocity: false);
    }

    protected override ListVH CreateViewsHolder(int itemIndex)
    {
        var vh = new ListVH();
        vh.Init(_Params.ItemPrefab, _Params.Content, itemIndex);
        vh.SetToggleDirectory(ToggleDirectory);
            
        if (onCreateItem != null)
        {
            onCreateItem(vh.item);
        }

        if (!listObjects.Contains(vh.item))
        {
            listObjects.Add(vh.item);
        }

        return vh;
    }

    protected override void UpdateViewsHolder(ListVH newOrRecycled)
    {
        var node = listInfo.all[newOrRecycled.ItemIndex];
        newOrRecycled.UpdateViews(node);

        /// 호출시 리스트 사이즈를 설정 가능
        ScheduleComputeVisibilityTwinPass();
    }

    /// <summary>
    /// 리스트 사이즈를 설정
    /// </summary>
    protected override float UpdateItemSizeOnTwinPass(ListVH viewsHolder)
    {
        return viewsHolder.UpdateSize();
        //return base.UpdateItemSizeOnTwinPass(viewsHolder);
    }

    private void ToggleDirectory(ListVH vh, Action<bool> onExpand)
    {
        int itemIndex = vh.ItemIndex;
        Node node = listInfo.all[itemIndex];
        if (!node._isDirectory)
        {
            return;
        }

        int nextIndex = itemIndex + 1;
        bool wasExpanded = node._isExpanded;
        node._isExpanded = !wasExpanded;
        if (wasExpanded)
        {
            // Remove all following models with bigger depth, until a model with a less than- or equal depth is found
            int i = itemIndex + 1;
            int count = listInfo.all.Count;
            //for (; i < count;)
            while (i < count)
            {
                var f = listInfo.all[i];
                if (f.depth > node.depth)
                {
                    f._isExpanded = false;
                    ++i;
                    continue;
                }

                break; // found with depth less than- or equal to the collapsed item
            }

            int countToRemove = i - nextIndex;
            if (countToRemove > 0)
            {
                listInfo.all.RemoveRange(nextIndex, countToRemove);
                RemoveItems(nextIndex, countToRemove);
            }
        }
        else
        {
            var aChild = node._childNodes;
            if (aChild != null && aChild.Count > 0)
            {
                listInfo.all.InsertRange(nextIndex, aChild);
                InsertItems(nextIndex, aChild.Count);
            }
        }

        if (vh != null)
        {
            vh.UpdateViews(node);
        }

        ForceUpdateViewsHolderIfVisible(itemIndex);

        if (onExpand != null)
        {
            onExpand(node._isExpanded);
        }
    }

    public void CollapseAll()
    {
        for (int i = 0; i < listInfo.all.Count;)
        {
            var m = listInfo.all[i];
            m._isExpanded = false;
            if (m.depth > 1)
            {
                listInfo.all.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
        ResetItems(listInfo.all.Count);
    }

    public void ExpandAll()
    {
        listInfo.all.Clear();
        listInfo.all.AddRange(listInfo.root.GetFlattenedHierarchyAndExpandAll());
        ResetItems(listInfo.all.Count);
    }

    public void Scroll(int idx, float viewStart = 0f, float pivot = 0f)
    {
        idx = Math.Max(0, idx);
        if (idx >= listInfo.all.Count)
        {
            return;
        }

        if (idx == 0)
        {
            ResetItems(0);
            ResetItems(listInfo.all.Count);
        }
        else
        {
            ScrollTo(idx, viewStart, pivot);
        }
    }


    public void Refresh()
    {
        for (int i = 0, cnt = listObjects.Count; i < cnt; ++i)
        {
            listObjects[i].Refresh();
        }
    }

    private class ListInfo
    {
        private Dictionary<IOsaItem, Node> dicRootItems = new Dictionary<IOsaItem, Node>();
        public List<Node> all { get; private set; } = new List<Node>();
        public Node root { get; private set; } = null;

        private static Node emptyRoot = Node.CreateRootItem(null);

        public void Set(IOsaItem rootItem)
        {
            Node root = null;
            if (rootItem != null)
            {
                if (!dicRootItems.TryGetValue(rootItem, out root))
                {
                    dicRootItems.Add(rootItem, root = Node.CreateRootItem(rootItem));
                }
            }
            else
            {
                root = emptyRoot;
            }

            all.Clear();
            void add(Node node)
            {
                all.Add(node);
                if (!node._isExpanded)
                {
                    return;
                }

                var children = node._childNodes;
                if (node._isSort)
                {
                    children.Sort((l, r) => { return l.item.SortCompare(r.item); });
                }

                foreach (var child in children)
                {
                    add(child);
                }
            }

            foreach (var node in root._childNodes)
            {
                add(node);
            }
            
            this.root = root;
        }
    }

    public class Node
    {
        public readonly Node parent = null;
        public readonly int depth = 0;
        public readonly IOsaItem item = null;

        private Node (Node parent, int depth, IOsaItem item)
        {
            this.parent = parent;
            this.depth = depth;
            this.item = item;
        }

        public bool _isExpanded
        {
            get { return item.isExpand; }
            set { item.isExpand = value; }
        }

        public bool _isSort
        {
            get
            {
                return item.isSort;
            }
        }

        public bool _isDirectory
        {
            get
            {
                return _childNodes.Count > 0;
            }
        }

        private List<Node> childNodes = null;
        public List<Node> _childNodes
        {
            get
            {
                if (childNodes == null)
                {
                    var temp = new List<Node>();
                    var children = item.children;
                    if (children != null)
                    {
                        foreach (var child in children)
                        {
                            temp.Add(new Node(this, depth + 1, child));
                        }
                    }

                    childNodes = temp;
                }

                return childNodes;
            }
        }

        private List<Node> temp = new List<Node>();
        public List<Node> GetFlattenedHierarchyAndExpandAll()
        {
            temp.Clear();
            foreach (var node in _childNodes)
            {
                temp.Add(node);
                node._isExpanded = true;
                if (node._isDirectory)
                {
                    temp.AddRange(node.GetFlattenedHierarchyAndExpandAll());
                }
            }

            return temp;
        }

        /// <summary>
        /// 최상위 Node 생성
        /// </summary>
        public static Node CreateRootItem(IOsaItem rootItem)
        {
            // 최상위 노드의 뎁스는 0
            return new Node(null, 0, rootItem);
        }
    }

    public class ListVH : BaseItemViewsHolder
    {
        public MyOSAHierarchyItem item { get; private set; } = null;

        public override void CollectViews()
        {
            base.CollectViews();

            if (root.TryGetComponent(out MyOSAHierarchyItem cp))
            {
                item = cp;
            }
            else
            {
                if (_DEBUG)
                {
                    Debug.Log("## MyOSAHierarchyItem component is null");
                }
            }
        }

        public void SetToggleDirectory(Action<ListVH, Action<bool>> onToggleDirectory)
        {
            item.SetToggleDirectory(this, onToggleDirectory);
        }

        public void UpdateViews(Node node)
        {
            if (item == null)
            {
                return;
            }

            item.DoReset();
            item.UpdateViews(node.item);
        }

        public float UpdateSize()
        {
            if (item == null)
            {
                return 0f;
            }

            var size = item.GetSize();
            root.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

            return size;
        }
    }

    public interface IOsaItem
    {
        List<IOsaItem> children { get; }
        bool isSort { get; }
        bool isExpand { get; set; }
        void CollapseAll();
        int SortCompare(IOsaItem other);
    }

#if USE_DEBUG
    private const bool _DEBUG = true;
#else
    private const bool _DEBUG = false;
#endif
}

