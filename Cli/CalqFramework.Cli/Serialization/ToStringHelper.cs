using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CalqFramework.Cli.Serialization.Parsing;

public static class ToStringHelper {
    public static string GetTypeName(Type type) {
        if (type.IsGenericType) {
            string genericArguments = string.Join(", ", type.GetGenericArguments().Select(x => x.Name.ToLower()).ToList());
            return $"{type.Name.ToLower().Substring(0, type.Name.ToLower().IndexOf("`"))}<{genericArguments}>";
        }
        return type.Name.ToLower();
    }

    // TODO option to turn on and off
    // FIXME handle FileNotFoundException
    public static string GetMemberSummary(MemberInfo memberInfo) {
        var xmlFilePath = Path.ChangeExtension(memberInfo.Module.Assembly.Location, "xml");

        string memberName;
        switch (memberInfo) {
            case MethodInfo method:
                memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
                var parameters = method.GetParameters();
                if (parameters.Length > 0) {
                    memberName += $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                }
                break;
            case PropertyInfo property:
                memberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
                break;
            case FieldInfo field:
                memberName = $"F:{field.DeclaringType?.FullName}.{field.Name}";
                break;
            default:
                throw new Exception("Unsupported member type.");
        }

        var doc = XDocument.Load(xmlFilePath);
        var summary = doc.Descendants("member")
            .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
            .Element("summary")?.Value;

        return $"{summary?.Trim()}\n" ?? "\n";
    }
}
