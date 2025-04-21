using System.Collections.Generic;
using System.Linq;


namespace MyPlayerComponent
{
    public class MyPlayerInventoryComponent : MyPlayerBaseComponent
    {
        public readonly MyPlayerInventoryEquipmentComponent equipment = null;
        public readonly MyPlayerInventoryAvatarComponent avatar = null;
        public readonly MyPlayerInventoryDictionaryComponent dictionary = null;

        private TInventory inventory = null;
        private int equipPoint = 0;

        public MyPlayerInventoryComponent(MyPlayer mp) : base(mp)
        {
            equipment = new MyPlayerInventoryEquipmentComponent(mp);
            avatar = new MyPlayerInventoryAvatarComponent(mp);
            dictionary = new MyPlayerInventoryDictionaryComponent(mp);
        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_INVENTORY_INFO, Handle_GET_INVENTORY_INFO)
                .Add(GameEventType.UPDATE_INVENTORY, Handle_UPDATE_INVENTORY)
                .Add(GameEventType.UPDATE_ITEM, Handle_UPDATE_ITEM)
                ;
        }

        private void Handle_GET_INVENTORY_INFO(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_INVENTORY_INFO>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateInventory(tArg.tinventory);
        }

        private void Handle_UPDATE_INVENTORY(object[] args)
        {
            var tinventory = GameEvent.GetSafe<TInventory>(args, 0);
            if (tinventory == null)
            {
                return;
            }

            UpdateInventory(tinventory);
        }

        private void Handle_UPDATE_ITEM(object[] args)
        {
            var titems = GameEvent.GetSafe<ICollection<TItem>>(args, 0);
            if (titems == null || titems.Count == 0)
            {
                return;
            }

            foreach (var item in titems)
            {
                if (!item.updateFlag.HasFlag(TItem.UpdateFlag.NEW))
                {
                    continue;
                }

                var resItem = item.resItem;
                if (resItem == null)
                {
                    continue;
                }

                if (!resItem.isFirstAutoEquip)
                {
                    continue;
                }

                EquipItem(resItem.id, true);
            }
        }

        private void UpdateInventory(TInventory _inventory)
        {
            if (inventory != null)
            {
                inventory.OnDisable();
            }

            inventory = _inventory;

            RefreshEquipPoint();
        }

        private void RefreshEquipPoint()
        {
            if (inventory == null)
            {
                return;
            }

            var point = 0;
            foreach (var resItemID in inventory.equipedItemIDs)
            {
                var resItem = ResourceManager.Instance.item.GetItem(resItemID);
                if (resItem == null)
                {
                    continue;
                }

                point += resItem.equipPoint;
            }

            equipPoint = point;
        }


        public bool IsEquipedItem(int resItemID)
        {
            if (inventory == null)
            {
                return false;
            }

            return inventory.equipedItemIDs.Contains(resItemID);
        }

        public int GetMaxEquipPoint()
        {
            return GameData.DEFAULT.MAX_ITEM_EQUIP_POINT;
        }

        public int GetEquipPoint()
        {
            return equipPoint;
        }

        public List<int> GetEquipedItemIDs()
        {
            if (inventory == null)
            {
                return new List<int>();
            }

            return inventory.equipedItemIDs;
        }

        public void EquipItem(int resItemID, bool isAuto = false)
        {
            if (inventory == null)
            {
                return;
            }

            var selectedResItem = ResourceManager.Instance.item.GetItem(resItemID);
            if (selectedResItem == null)
            {
                return;
            }

            var duplicationItem = mp.core.item.GetItems()
                .Where(x => inventory.equipedItemIDs.Contains(x.id))
                .FirstOrDefault(x =>
                {
                    var resItem = x.resItem;
                    if (resItem == null)
                    {
                        return false;
                    }

                    return selectedResItem.itemGroupID == resItem.itemGroupID;
                });

            if (duplicationItem != null)
            {
                if (!selectedResItem.isSwitch)
                {
                    if (!isAuto)
                    {
                        Main.Instance.ShowFloatingMessage("key_ex_item_equip_one".L());
                    }
                    return;
                }
            }

            if (equipPoint + selectedResItem.equipPoint > GetMaxEquipPoint())
            {
                if (!isAuto)
                {
                    Main.Instance.ShowFloatingMessage("key_ex_over_equippoint".L());
                }
                return;
            }

            VirtualServer.Send(Packet.EQUIP_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out EQUIP_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_INVENTORY, tArg.tinventory);
                    GameEvent.Instance.AddEvent(GameEventType.EQUIP_ITEM);
                },
                resItemID);
        }

        public void ReleaseItem(int resItemID)
        {
            VirtualServer.Send(Packet.RELEASE_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out EQUIP_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_INVENTORY, tArg.tinventory);
                    GameEvent.Instance.AddEvent(GameEventType.EQUIP_ITEM);
                },
                resItemID);
        }

        public void RerollItemOption(int resItemID)
        {
            VirtualServer.Send(Packet.REROLL_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out UPDATE_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                },
                resItemID);
        }

        public void EnhanceItem(int resItemID, int count)
        {
            VirtualServer.Send(Packet.ENHANCE_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out UPDATE_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                },
                resItemID,
                count);
        }

        public void EnhanceItemAll()
        {
            VirtualServer.Send(Packet.ENHANCE_ITEM_ALL,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out UPDATE_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                });
        }

        public void AwakenItem(int resItemID)
        {
            VirtualServer.Send(Packet.AWAKEN_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out UPDATE_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                },
                resItemID,
                1);
        }

        public void DismantleItem(int resItemID, long count)
        {
            VirtualServer.Send(Packet.DISMANTLE_ITEM,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out DISMANTLE_ITEM tArg))
                    {
                        return;
                    }

                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_ITEM, tArg.titems);
                    GameEvent.Instance.AddEvent(GameEventType.UPDATE_MISSION, tArg.tmissions);
                    GameEvent.Instance.AddEvent(GameEventType.SHOW_GET_INFO, tArg.getInfos);
                },
                resItemID,
                count);
        }
    }
}