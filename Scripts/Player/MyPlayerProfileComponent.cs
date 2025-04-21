namespace MyPlayerComponent
{
    public class MyPlayerProfileComponent : MyPlayerBaseComponent
    {
        public TUserInfo info { get; private set; } = null;

        public MyPlayerProfileComponent(MyPlayer mp) : base(mp)
        {

        }

        protected override EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return base.CreateHandler()
                .Add(GameEventType.GET_USER_INFO, Handle_GET_USER_INFO)
                .Add(GameEventType.CREATE_NICKNAME, Handle_CREATE_NICKNAME)
                ;
        }

        private void Handle_CREATE_NICKNAME(object[] args)
        {
            var tuserInfo = GameEvent.GetSafe<TUserInfo>(args, 0);
            if (tuserInfo == null)
            {
                return;
            }

            UpdateUserInfo(tuserInfo);
        }

        private void Handle_GET_USER_INFO(object[] args)
        {
            var tArg = GameEvent.GetSafe<GET_USER_INFO>(args, 0);
            if (tArg == null)
            {
                return;
            }

            UpdateUserInfo(tArg.tinfo);
        }

        private void UpdateUserInfo(TUserInfo _info)
        {
            if (info != null)
            {
                info.OnDisable();
            }

            info = _info;
        }

        public void ShowGuide(GuideType type)
        {
            UIGuide.CpUI_Guide.Instance.On(type, CompleteGuide);
        }

        public bool IsCompleteGuide(GuideType type)
        {
            var l = (long)type;
            return (info.guideFlag & l) == l;
        }

        private void CompleteGuide(GuideType type)
        {
            VirtualServer.Send(Packet.COMPLETE_GUIDE,
                (arg) =>
                {
                    if (!VirtualServer.TryGet(arg, out COMPLETE_GUIDE tArg))
                    {
                        return;
                    }

                    UpdateUserInfo(tArg.tinfo);
                },
                (long)type);
        }
    }
}
