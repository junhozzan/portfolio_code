using System.Collections.Generic;

public interface IInventory
{
    IEnumerable<ResourceItem> GetViewItems();
    bool TryGetItem(int id, out TItem item);

    int GetSelectedItemID();
    bool IsSelectedItem(int itemID);
    void SetSelectItem(int itemID);

    bool IsFiltering(ResourceItem resItem);
    void SetFilterType(ItemFilterType itemFilterType);
    ItemFilterType GetFilterType();
    ICollection<ItemFilterType> GetSortTypes();
}