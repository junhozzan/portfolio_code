using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UIMessageLabel
{
    public class CpUI_MessageLabel : MonoBehaviour
    {
        [SerializeField] CpUI_MessageLabelFrame labelFrame = null;

        private ObjectPool<CpUI_MessageLabelFrame> pool = null;

        public void Init()
        {
            pool = ObjectPool<CpUI_MessageLabelFrame>.Of(labelFrame, labelFrame.transform.parent);
        }

        public void On(string message)
        {
            gameObject.SetActive(true);

            // 이미 실행되고 있는 같은 메시지 찾기
            //var label = pool.GetPool().Find(x => x.gameObject.activeSelf && x.GetText() == message);
            //if (label == null)
            //{
            //    label = pool.Pop();
            //}
            
            var label = pool.Pop();
            label.SetText(message);
            label.transform.SetAsLastSibling();
        }
    }
}