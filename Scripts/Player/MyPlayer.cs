using MyPlayerComponent;

public class MyPlayer : Singleton<MyPlayer>
{
    public readonly MyPlayerCoreComponent core = null;

    public MyPlayer()
    {
        core = MyPlayerCoreComponent.Of(this);
    }

    public void Initialize()
    {
        core.Initialize();
    }
}
