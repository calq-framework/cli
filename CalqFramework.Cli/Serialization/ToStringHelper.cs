using CalqFramework.Cli.Attributes;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization.Parsing;

// TODO proper Serializer
public static class ToStringHelper {
    public static string GetTypeName(Type type) {
        if (type.IsGenericType) {
            string genericArguments = string.Join(", ", type.GetGenericArguments().Select(x => x.Name.ToLower()).ToList());
            return $"{type.Name.ToLower().Substring(0, type.Name.ToLower().IndexOf("`"))}<{genericArguments}>";
        }
        return type.Name.ToLower();
    }

    public static string? GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? $"({parameter.DefaultValue?.ToString()!.ToLower()})" : "";

    public static string MemberInfoToString(MemberInfo memberInfo) {
        var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
        // name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

        var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
        // shortName = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

        return ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}";
    }

    public static string ParameterToString(ParameterInfo parameterInfo) {
        return parameterInfo.Name!;
    }
}
