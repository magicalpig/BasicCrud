using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace BasicCrud.Enums.Helpers;
public static class DisplayNameHelper
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

    // had to abandon trying to return T? because that precluded me from returning null.
    // "Cannot convert null to type parameter 'T' because it could be a non-nullable value type"
    public static int? GetEnumValueFromDisplayName<T>(string displayName) where T : Enum
    {
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
            {
                if (string.Equals(attribute.Name, displayName, StringComparison.OrdinalIgnoreCase))
                {
                    return (int)field.GetValue(null)!;
                }
            }
        }
        return null;
    }
}
