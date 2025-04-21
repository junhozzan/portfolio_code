namespace MyPlayerComponent
{
    public abstract class MyPlayerBaseComponent : XComponent
    {
        protected readonly MyPlayer mp = null;

        private readonly EventDispatcher<GameEventType>.Handler handler = null;

        protected MyPlayerBaseComponent(MyPlayer mp)
        {
            this.mp = mp;
            this.handler = CreateHandler();
        }

        protected virtual void Dispose()
        {
            GameEvent.Instance.RemoveHandler(handler);
        }

        protected virtual EventDispatcher<GameEventType>.Handler CreateHandler()
        {
            return GameEvent.Instance.CreateHandler(this, null);
        }
    }
}
