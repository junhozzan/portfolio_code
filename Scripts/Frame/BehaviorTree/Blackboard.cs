using System.Collections.Generic;
using UnityEngine;
using System;

namespace behaviorTree
{
    public class BlackBoard
    {
        private readonly Dictionary<string, int> dicInt = new Dictionary<string, int>();
        private readonly Dictionary<string, int> dicKeepInt = new Dictionary<string, int>();
        private readonly Dictionary<string, long> dicLong = new Dictionary<string, long>();
        private readonly Dictionary<string, long> dicKeepLong = new Dictionary<string, long>();
        private readonly Dictionary<string, float> dicFloat = new Dictionary<string, float>();
        private readonly Dictionary<string, float> dicKeepFloat = new Dictionary<string, float>();
        private readonly Dictionary<string, string> dicString = new Dictionary<string, string>();
        private readonly Dictionary<string, string> dicKeepString = new Dictionary<string, string>();
        private readonly Dictionary<string, bool> dicBool = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> dicKeepBool = new Dictionary<string, bool>();
        private readonly Dictionary<string, Vector2> dicVector2 = new Dictionary<string, Vector2>();
        private readonly Dictionary<string, Vector2> dicKeepVector2 = new Dictionary<string, Vector2>();
        private readonly Dictionary<string, Vector2Int> dicVector2Int = new Dictionary<string, Vector2Int>();
        private readonly Dictionary<string, Vector2Int> dicKeepVector2Int = new Dictionary<string, Vector2Int>();
        private readonly Dictionary<string, DateTime> dicDateTime = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, DateTime> dicKeepDateTime = new Dictionary<string, DateTime>();


        public static BlackBoard Of()
        {
            return new BlackBoard();
        }

        public virtual void Clear()
        {
            dicInt.Clear();
            dicLong.Clear();
            dicFloat.Clear();
            dicString.Clear();
            dicBool.Clear();
            dicDateTime.Clear();
        }

        #region SET
        public void SetInt(string key, int v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepInt : dicInt;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 0);
            }

            dic[key] = v;
        }

        public void SetLong(string key, long v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepLong : dicLong;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 0);
            }

            dic[key] = v;
        }

        public void SetFloat(string key, float v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepFloat : dicFloat;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 0);
            }

            dic[key] = v;
        }

        public void SetString(string key, string v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepString : dicString;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, string.Empty);
            }

            dic[key] = v;
        }

        public void SetBool(string key, bool v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepBool : dicBool;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, false);
            }

            dic[key] = v;
        }

        public void SetDateTime(string key, DateTime v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepDateTime : dicDateTime;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, DateTime.MinValue);
            }

            dic[key] = v;
        }

        public void SetVector2(string key, Vector2 v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepVector2 : dicVector2;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, Vector2.zero);
            }

            dic[key] = v;
        }

        public void SetVector2Int(string key, Vector2Int v, bool keep = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var dic = keep ? dicKeepVector2Int : dicVector2Int;
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, Vector2Int.zero);
            }

            dic[key] = v;
        }
        #endregion SET

        #region GET
        public int? GetInt(string key, int? _default = null)
        {
            if (dicInt.TryGetValue(key, out var v) || dicKeepInt.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public long? GetLong(string key, long? _default = null)
        {
            if (dicLong.TryGetValue(key, out var v) || dicKeepLong.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public float? GetFloat(string key, float? _default = null)
        {
            if (dicFloat.TryGetValue(key, out var v) || dicKeepFloat.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public string GetString(string key, string _default = "")
        {
            if (dicString.TryGetValue(key, out var v) || dicKeepString.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public bool GetBool(string key, bool _default = false)
        {
            if (dicBool.TryGetValue(key, out var v) || dicKeepBool.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public DateTime? GetDateTime(string key, DateTime? _default = null)
        {
            if (dicDateTime.TryGetValue(key, out var v) || dicKeepDateTime.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public Vector2? GetVector2(string key, Vector2? _default = null)
        {
            if (dicVector2.TryGetValue(key, out var v) || dicKeepVector2.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }

        public Vector2Int? GetVector2Int(string key, Vector2Int? _default = null)
        {
            if (dicVector2Int.TryGetValue(key, out var v) || dicKeepVector2Int.TryGetValue(key, out v))
            {
                return v;
            }

            return _default;
        }
        #endregion GET

        #region ADD
        public void AddInt(string key, int add)
        {
            var v = GetInt(key);
            if (v.HasValue)
            {
                var value = v.Value;
                try
                {
                    value = checked(value + add);
                }
                catch
                {
                    value = add >= 0 ? int.MaxValue : int.MinValue;
                    if (_DEBUG)
                    {
                        Debug.Log("## blackboard int flow catch ! ");
                    }
                }

                SetInt(key, value);
            }
            else
            {
                SetInt(key, add);
            }
        }

        public void AddLong(string key, long add)
        {
            var v = GetLong(key);
            if (v.HasValue)
            {
                var value = v.Value;
                try
                {
                    value = checked(value + add);
                }
                catch
                {
                    value = add >= 0 ? long.MaxValue : long.MinValue;
                    if (_DEBUG)
                    {
                        Debug.Log("## blackboard long flow catch ! ");
                    }
                }

                SetLong(key, value);
            }
            else
            {
                SetLong(key, add);
            }
        }

        public void AddFloat(string key, float add)
        {
            var v = GetFloat(key);
            if (v.HasValue)
            {
                var value = v.Value;
                try
                {
                    value = checked(value + add);
                }
                catch
                {
                    value = add >= 0 ? float.MaxValue : float.MinValue;
                    if (_DEBUG)
                    {
                        Debug.Log("## blackboard float flow catch ! ");
                    }
                }

                SetFloat(key, value);
            }
            else
            {
                SetFloat(key, add);
            }
        }
        #endregion ADD


#if USE_DEBUG
        protected const bool _DEBUG = true;
#else
        protected const bool _DEBUG = false;
#endif
    }
}