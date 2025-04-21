namespace UICollection
{
    public class CellInfo
    {
        public MyOSAHierarchy.IOsaItem osaItem { get; private set; } = null;

        public ResourceCollectionGroup resCollectionGroup { get; private set; } = null;
        public ResourceCollection resCollection { get; private set; } = null;

        public void SetOsaItem(MyOSAHierarchy.IOsaItem osaItem)
        {
            this.osaItem = osaItem;

            SetDefault();
            Refresh();
        }

        private void SetDefault()
        {
            resCollectionGroup = null;
            resCollection = null;
        }

        public void Refresh()
        {
            if (osaItem is GroupOsaItem groupOsaItem)
            {
                resCollectionGroup = groupOsaItem.resCollectionGroup;
            }
            else if (osaItem is ContentOsaItem contentOsaItem)
            {
                resCollection = contentOsaItem.resCollection;
            }
        }
    }
}
