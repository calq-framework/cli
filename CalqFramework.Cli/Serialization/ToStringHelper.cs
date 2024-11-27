using CalqFramework.Cli.Attributes;
using CalqFramework.Extensions.System.Reflection;
using CalqFramework.Serialization.DataAccess;
using System;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization.Parsing;

public static class ToStringHelper {
    public static string GetTypeName(Type type) {
        if (type.IsGenericType) {
            string genericArguments = string.Join(", ", type.GetGenericArguments().Select(x => x.Name.ToLower()).ToList());
            return $"{type.Name.ToLower().Substring(0, type.Name.ToLower().IndexOf("`"))}<{genericArguments}>";
        }
        return type.Name.ToLower();
    }

    public static string? GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? $"({parameter.DefaultValue?.ToString()!.ToLower()})" : "";

    public static string ParameterToString(ParameterInfo parameterInfo) {
        return parameterInfo.Name!;
    }
}
