using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UICollection
{
    public class TreeOsaItem : MyOSAHierarchy.IOsaItem
    {
        protected List<MyOSAHierarchy.IOsaItem> _children = null;
        public List<MyOSAHierarchy.IOsaItem> children
        {
            get
            {
                return _children;
            }
        }

        public bool isExpand { get; set; } = false;
        public virtual bool isSort { get; } = false;

        public void CollapseAll()
        {
            isExpand = false;

            if (children == null)
            {
                return;
            }

            foreach (var child in children)
            {
                child.CollapseAll();
            }
        }

        public int SortCompare(MyOSAHierarchy.IOsaItem other)
        {
            return 0;
        }
    }

    public class RootOsaItem : TreeOsaItem
    {
        public static RootOsaItem Of(IEnumerable<GroupOsaItem> children)
        {
            return new RootOsaItem(children);
        }

        protected RootOsaItem(IEnumerable<GroupOsaItem> children)
        {
            this._children = new List<MyOSAHierarchy.IOsaItem>(children);
        }
    }

    public class GroupOsaItem : TreeOsaItem
    {
        public readonly ResourceCollectionGroup resCollectionGroup = null;

        public static GroupOsaItem Of(ResourceCollectionGroup resCollectionGroup)
        {
            return new GroupOsaItem(resCollectionGroup);
        }

        protected GroupOsaItem(ResourceCollectionGroup resCollectionGroup)
        {
            this.resCollectionGroup = resCollectionGroup;
            this._children = new List<MyOSAHierarchy.IOsaItem>(
                resCollectionGroup.collectionIDs
                .Select(x => ResourceManager.Instance.collection.GetCollection(x))
                .Select(x => ContentOsaItem.Of(x))
            );
        }
    }

    public class ContentOsaItem : TreeOsaItem
    {
        public readonly ResourceCollection resCollection = null;

        public static ContentOsaItem Of(ResourceCollection resCollection)
        {
            return new ContentOsaItem(resCollection);
        }

        protected ContentOsaItem(ResourceCollection resCollection)
        {
            this.resCollection = resCollection;
        }
    }
}
