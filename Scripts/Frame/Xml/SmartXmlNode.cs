using System;
using System.Collections.Generic;
using System.Xml;

public struct TXMLAttribute {
    public string name;
    public string text;
}

public class TXMLElement {
    public string name;
    public string text;

    public TXMLAttribute[] attributes;
    public List<TXMLElement> children = new List<TXMLElement>();

    public TXMLElement() {}

    public TXMLElement(List<TXMLElement> children) {
        if (children == null || children.Count == 0)
        {
            return;
        }

        this.children.AddRange(children);
    }

    public static TXMLElement empty = new TXMLElement();
}

public class SmartXmlNodePooler
{
    static List<SmartXmlNode> activesmartXmlNodes = new List<SmartXmlNode>();

    public static void AddSmartXmlNode(SmartXmlNode xmlNode)
    {
        activesmartXmlNodes.Add(xmlNode);
    }

    public static void Clear()
    {
        foreach (SmartXmlNode xmlNode in activesmartXmlNodes)
            xmlNode.Clear(true);

        activesmartXmlNodes.Clear();
    }

}
public class SmartXmlNode
{
    private Dictionary<string, List<TXMLElement>> _fastChildren;
    private Dictionary<string, List<TXMLAttribute>> _fastAttributes;

    public string id;

    int referenceCount = 1;

    public SmartXmlNode Get()
    {
        ++referenceCount;

        return this;
    }

    public SmartXmlNode(XmlReader reader)
    {
        _fastChildren = new Dictionary<string, List<TXMLElement>>();
        _fastAttributes = new Dictionary<string, List<TXMLAttribute>>();

        for (int i = 0; i < reader.AttributeCount; ++i)
        {
            reader.MoveToAttribute(i);

            if (reader.Name == "id")
                this.id = reader.Value;

            if (!_fastAttributes.TryGetValue(reader.Name, out var lst))
                _fastAttributes.Add(reader.Name, lst = new List<TXMLAttribute>());

            lst.Add(new TXMLAttribute() { text = reader.Value });
        }

        if (reader.IsEmptyElement)
            return;

        Stack<TXMLElement> stack = new Stack<TXMLElement>();
        TXMLElement lastElement = null;
        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    bool isEmptyElement = reader.IsEmptyElement;

                    var e = new TXMLElement()
                    {
                        name = reader.Name,
                        text = reader.Value,
                    };

                    // Attributes 넣기
                    if (reader.AttributeCount > 0)
                    {
                        e.attributes = new TXMLAttribute[reader.AttributeCount];
                        for (int i = 0; i < reader.AttributeCount; ++i)
                        {
                            reader.MoveToAttribute(i);

                            e.attributes[i] = new TXMLAttribute()
                            {
                                name = reader.Name,
                                text = reader.Value,
                            };
                        }
                    }

                    if (lastElement == null)
                    {
                        if (!_fastChildren.TryGetValue(e.name, out var lst))
                            _fastChildren[e.name] = lst = new List<TXMLElement>();

                        lst.Add(e);
                    }
                    else
                    {
                        lastElement.children.Add(e);
                    }

                    if (!isEmptyElement)
                    {
                        lastElement = e;
                        stack.Push(e);
                    }
                    break;

                case XmlNodeType.Text:
                    if (lastElement != null)
                        lastElement.text = reader.Value;
                    break;

                case XmlNodeType.CDATA:
                    if (lastElement != null)
                        lastElement.text = reader.Value;
                    break;

                case XmlNodeType.EndElement:
                    if (lastElement == null)
                        return;

                    stack.Pop();
                    lastElement = stack.Count > 0 ? stack.Peek() : null;
                    break;

                default:
                    break;
            }
        }
    }

    public void Clear(bool isForce = false)
    {
        if (isForce == false)
        {
            --referenceCount;
            if (referenceCount > 0)
                return;
        }

        foreach (var it in _fastChildren)
        {
            foreach (var it2 in it.Value)
            {
                var arr = it2.attributes;
                if (arr != null)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = default;
                    }
                }

                it2.children?.Clear();
            }

            it.Value.Clear();
        }


        _fastChildren.Clear();

        foreach (var it in _fastAttributes)
            it.Value.Clear();

        _fastAttributes.Clear();

    }
    public object GetChildEnum(string key, object _default)
    {
        var value = GetChildText(key);
        if (string.IsNullOrEmpty(value))
            return _default;

        try
        {
            return Enum.Parse(_default.GetType(), value, true);
        }
        catch (Exception)
        {
            return _default;
        }
    }

    public T GetChildEnum<T>(string key, T _default = default(T)) where T : struct
    {
        var value = GetChildText(key);
        if (string.IsNullOrEmpty(value))
            return _default;

        if (Enum.TryParse<T>(value, true, out var result))
            return result;
        return _default;
    }

    //public T GetChildEnumInt<T>(string key, T _default = default(T)) where T : struct
    //{
    //    var value = GetChildInt(key);
    //    if (!typeof(T).IsEnum)
    //    {
    //        return _default;
    //    }

    //    return (T)(object)value;
    //}

    public string GetChildText(string key, string _default = null)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _default;
        return lst[0].text.Replace("\\n", "\n");
    }

    public bool GetChildBoolean(string key, bool _default = false)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _default;

        if (bool.TryParse(lst[0].text, out var result))
            return result;

        return _default;
    }

    public int GetChildInt(string key, int _default = 0)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _default;

        if (int.TryParse(lst[0].text, out var result))
            return result;

        return _default;
    }

    public long GetChildLong(string key, long _default = 0)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _default;

        if (long.TryParse(lst[0].text, out var result))
            return result;

        return _default;
    }

    public float GetChildFloat(string key, float _default = 0f)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _default;

        if (float.TryParse(lst[0].text, out var result))
            return result;

        return _default;
    }

    public string GetAttributeText(string key)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return null;
        return lst[0].text;
    }

    public string GetAttributeText(string key, string defaultStr)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return defaultStr;
        return lst[0].text;
    }

    public int GetAttributeInt(string key, int _default = 0)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return _default;
        return int.Parse(lst[0].text);
    }

    public float GetAttributeFloat(string key, float _default = 0)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return _default;
        return float.Parse(lst[0].text);
    }

    public double GetAttributeDouble(string key, double _default = 0)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return _default;
        return double.Parse(lst[0].text);
    }

    public long GetAttributeLong(string key, long _default = 0)
    {
        if (!_fastAttributes.TryGetValue(key, out var lst))
            return _default;
        return long.Parse(lst[0].text);
    }

    public T GetAttributeEnum<T>(string key, T _default = default) where T : struct
    {
        var str = GetAttributeText(key);
        if (string.IsNullOrEmpty(str)
            || !typeof(T).IsEnum)
        {
            return _default;
        }

        return Enum.TryParse<T>(str, out var value) ? value : _default;
    }

    public T GetAttributeEnumInt<T>(string key, T _default = default) where T : struct
    {
        var value = GetAttributeInt(key);
        if (!typeof(T).IsEnum)
        {
            return _default;
        }

        return (T)(object)value;
    }

    private static readonly List<TXMLElement> _emptyList = new List<TXMLElement>();

    public List<TXMLElement> GetChildren(string key)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return _emptyList;
        return lst;
    }

    public TXMLElement GetChild(string key)
    {
        if (!_fastChildren.TryGetValue(key, out var lst))
            return null;
        return lst[0];
    }
}

public static class SmartXmlNodeExtensions
{
    public static T GetChildEnum<T>(this TXMLElement node, string key, T _default = default(T)) where T : struct
    {
        var value = node.GetChildText(key);
        if (string.IsNullOrEmpty(value))
            return _default;

        if (Enum.TryParse<T>(value, true, out var result))
            return result;
        return _default;
    }

    //public static T GetChildEnumInt<T>(this TXMLElement node, string key, T _default = default(T)) where T : struct
    //{
    //    var value = node.GetChildInt(key);
    //    if (!typeof(T).IsEnum)
    //    {
    //        return _default;
    //    }

    //    return (T)(object)value;
    //}

    public static string GetChildText(this TXMLElement node, string key, string _default = null)
    {
        if (node.children == null)
            return _default;

        node = node.children.Find(x => x.name == key);
        if (node == null)
            return _default;
        return node.text.Replace("\\n", "\n");
    }

    public static bool GetChildBoolean(this TXMLElement node, string key, bool _default = false)
    {
        node = node.GetChild(key);
        if (node == null)
            return _default;
        return bool.Parse(node.text);
    }

    public static int GetChildInt(this TXMLElement node, string key, int _default = 0)
    {
        node = node.GetChild(key);
        if (node == null)
            return _default;
        return int.Parse(node.text);
    }

    public static long GetChildLong(this TXMLElement node, string key, long _default = 0)
    {
        node = node.GetChild(key);
        if (node == null)
            return _default;
        return long.Parse(node.text);
    }

    public static float GetChildFloat(this TXMLElement node, string key, float _default = 0f)
    {
        node = node.GetChild(key);
        if (node == null)
            return _default;
        return float.Parse(node.text);
    }

    public static double GetChildDouble(this TXMLElement node, string key, double _default = 0d)
    {
        node = node.GetChild(key);
        if (node == null)
            return _default;
        return double.Parse(node.text);
    }

    private static List<TXMLElement> _emptyList = new List<TXMLElement>();
    public static List<TXMLElement> GetChildren(this TXMLElement node, string key)
    {
        if (node.children == null)
            return _emptyList;

        var lst = new List<TXMLElement>();
        for (int i = 0; i < node.children.Count; ++i)
        {
            if (node.children[i].name == key)
                lst.Add(node.children[i]);
        }

        return lst;
    }

    public static TXMLElement GetChild(this TXMLElement node, string key)
    {
        if (node.children == null)
            return null;

        return node.children.Find(x => x.name == key);
    }

    public static string GetAttributeText(this TXMLElement node, string key, string _default = null)
    {
        if (node.attributes == null)
            return _default;

        for (int i = 0; i < node.attributes.Length; ++i)
        {
            if (node.attributes[i].name == key)
                return node.attributes[i].text;
        }

        return _default;
    }

    public static T GetAttributeEnum<T>(this TXMLElement node, string key, T _default = default(T)) where T : struct
    {
        var value = node.GetAttributeText(key);
        if (string.IsNullOrEmpty(value))
            return _default;

        if (Enum.TryParse<T>(value, true, out var result))
            return result;

        return _default;
    }

    //public static T GetAttributeEnumInt<T>(this TXMLElement node, string key, T _default = default) where T : struct
    //{
    //    var value = node.GetAttributeInt(key);
    //    if (!typeof(T).IsEnum)
    //    {
    //        return _default;
    //    }

    //    return (T)(object)value;
    //}

    public static int GetAttributeInt(this TXMLElement node, string key, int _default = 0)
    {
        var text = node.GetAttributeText(key);
        if (text == null)
            return _default;
        return int.Parse(text);
    }

    public static int[] GetAttributeArrayInt(this TXMLElement node, string key, int _default = 0)
    {
        var text = node.GetAttributeText(key);
        var list = new List<int>();
        if (text == null)
        {
            list.Add(_default);
            return list.ToArray();
        }
        var args = text.Split(',');
        foreach (string str in args)
        {
            list.Add(int.Parse(str));
        }
        return list.ToArray();
    }

    public static bool GetAttributeBoolean(this TXMLElement node, string key, bool _default = false)
    {
        var text = node.GetAttributeText(key);
        if (text == null)
            return _default;
        return bool.Parse(text);
    }

    public static float GetAttributeFloat(this TXMLElement node, string key, float _default = 0)
    {
        var text = node.GetAttributeText(key);
        if (text == null)
            return _default;
        return float.Parse(text);
    }

    public static double GetAttributeDouble(this TXMLElement node, string key, double _default = 0)
    {
        var text = node.GetAttributeText(key);
        if (text == null)
            return _default;
        return double.Parse(text);
    }

    public static long GetAttributeLong(this TXMLElement node, string key, long _default = 0)
    {
        var text = node.GetAttributeText(key);
        if (text == null)
            return _default;
        return long.Parse(text);
    }
}