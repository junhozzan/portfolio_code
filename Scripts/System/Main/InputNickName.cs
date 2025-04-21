using System.Collections;

public class InputNickName
{
    public IEnumerator ShowInputNickName()
    {
        if (string.IsNullOrEmpty(User.Instance.info.nickName))
        {
            var popup = PopupExtend.Instance.ShowInputNickName();

            while (popup.gameObject.activeSelf || string.IsNullOrEmpty(User.Instance.info.nickName))
            {
                yield return null;
            }
        }
    }
}
