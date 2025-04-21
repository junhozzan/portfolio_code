namespace MyPlayerComponent
{
    public class MyPlayerCoreComponent : MyPlayerBaseComponent
    {
        public readonly MyPlayerProfileComponent profile = null;
        public readonly MyPlayerItemComponent item = null;
        public readonly MyPlayerInventoryComponent inventory = null;
        public readonly MyPlayerModeComponent mode = null;
        public readonly MyPlayerLabComponent lab = null;
        public readonly MyPlayerMissionComponent mission = null;
        public readonly MyPlayerAttendanceComponent attendance = null;
        public readonly MyPlayerCollectionComponent collection = null;
        public readonly MyPlayerStoreComponent store = null;
        public readonly MyPlayerAdvertiseComponent ad = null;
        public readonly MyPlayerCardComponent card = null;
        public readonly MyPlayerMergeComponent merge = null;

        public static MyPlayerCoreComponent Of(MyPlayer mp)
        {
            return new MyPlayerCoreComponent(mp);
        }

        private MyPlayerCoreComponent(MyPlayer mp) : base(mp)
        {
            profile = AddComponent<MyPlayerProfileComponent>(mp);
            item = AddComponent<MyPlayerItemComponent>(mp);
            inventory = AddComponent<MyPlayerInventoryComponent>(mp);
            mode = AddComponent<MyPlayerModeComponent>(mp);
            lab = AddComponent<MyPlayerLabComponent>(mp);
            mission = AddComponent<MyPlayerMissionComponent>(mp);
            attendance = AddComponent<MyPlayerAttendanceComponent>(mp);
            collection = AddComponent<MyPlayerCollectionComponent>(mp);
            store = AddComponent<MyPlayerStoreComponent>(mp);
            ad = AddComponent<MyPlayerAdvertiseComponent>(mp);
            card = AddComponent<MyPlayerCardComponent>(mp);
            merge = AddComponent<MyPlayerMergeComponent>(mp);
        }
    }
}