using System;
using System.Linq;

public static class SmartXmlUtility
{
    public static void ParseSmart(this object o, SmartXmlNode e)
    {
        var attr = typeof(SmartXmlAttribute);
        foreach (var f in o.GetType().GetFields())
        {
            var a = f.GetCustomAttributes(attr, false).FirstOrDefault() as SmartXmlAttribute;
            if (a == null)
                continue;

            var xmlName = a.childName;
            if (xmlName == null)
                xmlName = f.Name.Substring(0, 1).ToUpper() + f.Name.Substring(1);

            var fieldType = f.FieldType;

            if (fieldType == typeof(int))
                f.SetValue(o, e.GetChildInt(xmlName, a.GetValue<int>()));

            if (fieldType == typeof(long))
                f.SetValue(o, e.GetChildLong(xmlName, a.GetValue<long>()));

            else if (fieldType == typeof(float))
                f.SetValue(o, e.GetChildFloat(xmlName, a.GetValue<float>()));

            else if (fieldType == typeof(string))
                f.SetValue(o, e.GetChildText(xmlName, a.GetValue<string>()));

            else if (fieldType == typeof(bool))
                f.SetValue(o, e.GetChildBoolean(xmlName, a.GetValue<bool>()));

            else if (fieldType.IsEnum)
                f.SetValue(o, e.GetChildEnum(xmlName, a.GetValue(fieldType)));

            else
            {

            }
        }
    }
}

class SmartXmlAttribute : Attribute
{
    public readonly string childName;

    public object defaultValue;

    public SmartXmlAttribute(object defaultValue = null, string childName = null)
    {
        this.childName = childName;

        this.defaultValue = defaultValue;
    }

    public T GetValue<T>()
    {
        if (defaultValue is T)
            return (T)defaultValue;

        try
        {
            return (T)Convert.ChangeType(defaultValue, typeof(T));
        }
        catch
        {
            return default(T);
        }
    }

    public object GetValue(Type type)
    {
        if (defaultValue == null || defaultValue.GetType() == type)
            return defaultValue;

        try
        {
            return Convert.ChangeType(defaultValue, type);
        }
        catch
        {
            return null;
        }
    }
}
