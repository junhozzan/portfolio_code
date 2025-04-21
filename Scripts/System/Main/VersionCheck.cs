using SimpleJSON;
using UnityEngine;
using System.Collections;

public class VersionCheck
{
    public string productVersion { get; private set; } = string.Empty;

    public VersionCheck()
    {

    }

    public IEnumerator LoadProductVersion()
    {
        if (!IsNetworkConnected())
        {
            yield break;
        }

        var loadComplete = false;
        GGSheetManager.Instance.Get(GameData.GGSHEET_URL.main, (result, text) =>
        {
            loadComplete = true;

            if (result != GGSheetManager.NetworkState.SUCCESS)
            {
                return;
            }

            var node = JSONNode.Parse(text);
            var version = JSONCtrl.GetString(node, "version");

            SetProductVersion(version);
        });

        // 패킷 최소시간을 설정해서 진입을 빠르게 한다.
        var maxTime = 2.5f;
        while (!loadComplete)
        {
            maxTime -= Time.deltaTime;
            if (maxTime <= 0f)
            {
                break;
            }

            yield return null;
        }
    }

    private void SetProductVersion(string version)
    {
        productVersion = version;
    }

    public IEnumerator ShowVersionCheck()
    {
        if (Option.checkedVersion != productVersion && IsAppLowerVersion())
        {
            var popup = PopupExtend.Instance.ShowVersionChecker(productVersion);
            while (popup.gameObject.activeSelf)
            {
                yield return null;
            }
        }
    }

    public bool IsAppLowerVersion()
    {
        if (Option.checkedVersion == productVersion)
        {
            return false;
        }

        var _appVersion = Application.version.ToDECIMAL();
        var _productVersion = productVersion.ToDECIMAL();
        if (_DEBUG)
        {
            Debug.Log($"## AppVersion: {_appVersion}, ProductVersion: {_productVersion}");
        }

        return _productVersion > _appVersion;
    }

    private bool IsNetworkConnected()
    {
        if (_DEBUG)
        {
            Debug.Log($"## Connected State {Application.internetReachability}");
        }

        return Application.internetReachability != NetworkReachability.NotReachable;
    }

#if USE_DEBUG
    protected const bool _DEBUG = true;
#else
    protected const bool _DEBUG = false;
#endif
}
