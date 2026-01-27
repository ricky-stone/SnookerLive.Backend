using System.Reflection;

namespace Comparer;

public static class ModelComparer
{
    public static List<PropertyDifference> Compare<T>(T? obj1, T? obj2)
    {
        return CompareObjects(obj1, obj2);
    }

    private static List<PropertyDifference> CompareObjects(object? obj1, object? obj2)
    {
        var differences = new List<PropertyDifference>();

        if (obj1 == null && obj2 == null) return differences;
        if (obj1 == null || obj2 == null)
        {
            differences.Add(new PropertyDifference
            {
                PropertyName = "Object",
                Value1 = obj1,
                Value2 = obj2
            });
            return differences;
        }

        var type = obj1.GetType();
        if (type != obj2.GetType())
        {
            differences.Add(new PropertyDifference
            {
                PropertyName = "Type",
                Value1 = type,
                Value2 = obj2.GetType()
            });
            return differences;
        }

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanRead) continue;

            var value1 = prop.GetValue(obj1);
            var value2 = prop.GetValue(obj2);

            if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string))
            {
                var nested = CompareObjects(value1, value2);
                foreach (var diff in nested)
                {
                    diff.PropertyName = $"{prop.Name}.{diff.PropertyName}";
                    differences.Add(diff);
                }
            }
            else if (!Equals(value1, value2))
            {
                differences.Add(new PropertyDifference
                {
                    PropertyName = prop.Name,
                    Value1 = value1,
                    Value2 = value2
                });
            }
        }

        return differences;
    }
}