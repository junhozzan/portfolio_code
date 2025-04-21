using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIMode
{
    public class CpUI_Mode : UIMonoBehaviour
    {
        private static CpUI_Mode instance = null;
        public static CpUI_Mode Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = UIManager.Instance.Find<CpUI_Mode>("pf_ui_mode");
                }

                return instance;
            }
        }

        [SerializeField] CpUI_MonsterHpSlider monsterHpSlider = null;
        [SerializeField] UIText scoreText = null;
        [SerializeField] UIText levelText = null;
        [SerializeField] UIText dpsText = null;
        [SerializeField] UIText timeText = null;
        [SerializeField] Image expGage = null;
        [SerializeField] UIText getItemText = null;
        [SerializeField] UIText bestText = null;

        [SerializeField] UISlider playerHpSlider = null;
        [SerializeField] UISlider playerMpSlider = null;

        private ObjectPool<UIText> getItemTextPool = null;

        public override void Init()
        {
            base.Init();
            SetCanvas(UIManager.eCanvans.BASE, true);
            SetDpsText("0");
            SetBestScoreText("0");
            SetScoreText(string.Empty);

            getItemTextPool = ObjectPool<UIText>.Of(getItemText, getItemText.transform.parent);
        }

        public void On()
        {
            SetDefault();
            UIManager.Instance.Show(this);
        }

        public void SetDefault()
        {
            InactiveTimeText();
            InactiveMonsterHPBar();
        }

        private void InactiveTimeText()
        {
            timeText.gameObject.SetActive(false);
        }

        private void InactiveMonsterHPBar()
        {
            monsterHpSlider.DoReset();
            monsterHpSlider.gameObject.SetActive(false);
        }

        public void ShowGetItems(long addGold, IList<GetInfo> getInfos)
        {
            if (addGold > 0)
            {
                var resGoldItem = ResourceManager.Instance.item.GetItem(GameData.ITEM_DATA.GOLD);
                if (resGoldItem != null)
                {
                    var text = getItemTextPool.Pop();
                    text.SetTextColor(resGoldItem.GetGradeColor());
                    text.SetText($"{resGoldItem.GetName()} + {Util.ToComma(addGold)}");
                }
            }

            if (getInfos != null)
            {
                foreach (var getInfo in getInfos)
                {
                    var resItem = ResourceManager.Instance.item.GetItem(getInfo.itemID);
                    if (resItem == null)
                    {
                        continue;
                    }

                    var text = getItemTextPool.Pop();
                    text.SetTextColor(resItem.GetGradeColor());
                    text.SetText($"{resItem.GetName()} +{Util.ToComma(getInfo.amount)}");
                }
            }
        }

        public void SetScoreText(string s)
        {
            scoreText.SetText(s);
        }

        public void SetLevel(long level, float rate)
        {
            levelText.SetText($"{"key_level".L()} {level} ({rate * 100f:0.00}%)");
        }

        public void SetMonsterHpSlider(bool firstHit, long unitUID, float maxHp, float hp, float prevHp, float countValue, float hpBarSize)
        {
            monsterHpSlider.gameObject.SetActive(true);
            monsterHpSlider.SetShadowFill(firstHit, unitUID, maxHp, prevHp, hp, countValue, hpBarSize);
        }

        public void SetDpsText(string text)
        {
            dpsText.SetText(text);
        }

        public void SetBestScoreText(string text)
        {
            bestText.SetText(text);
        }

        public void SetTimeText(string s)
        {
            timeText.gameObject.SetActive(true);
            timeText.SetText(s);
        }

        public void SetExpGage(float rate)
        {
            expGage.fillAmount = rate;
        }

        public void SetMainUnitHp(float fill)
        {
            playerHpSlider.SetFill(fill);
        }

        public void SetMainUnitMp(float fill)
        {
            playerMpSlider.SetFill(fill);
        }

        public override bool CanClose()
        {
            PopupExtend.Instance.ShowQuit();
            return false;
        }

        private void Cmd_OpenCardSelectPopup()
        {
            ModeManager.Instance.mode.core.ui.card.OpenCardSelect();
        }
    }
}