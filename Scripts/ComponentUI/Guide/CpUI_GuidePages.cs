using UnityEngine;

namespace UIGuide
{
    public class CpUI_GuidePages : MonoBehaviour
    {
        private GameObject[] childs = null;
        public GameObject[] _childs
        {
            get
            {
                if (childs == null)
                {
                    childs = new GameObject[transform.childCount];
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        childs[i] = transform.GetChild(i).gameObject;
                    }
                }

                return childs;
            }
        }

        public void SetPage(int index)
        {
            if (index < 0 || index >= _childs.Length)
            {
                return;
            }

            for (int i = 0; i < _childs.Length; ++i)
            {
                _childs[i].SetActive(i == index);
            }
        }

        public bool IsAimationPlaying()
        {
            return false;
        }

        public bool IsFinal(int index)
        {
            return index >= _childs.Length - 1;
        }
    }
}