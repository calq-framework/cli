using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Formatting;

public abstract class ClassMemberStringifierBase : IClassMemberStringifier {
    public IEnumerable<string> GetAlternativeNames(FieldInfo info) =>
        GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetAlternativeNames(MethodInfo info) =>
        GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetAlternativeNames(ParameterInfo info) =>
        GetAlternativeNames(info.Name!, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetAlternativeNames(PropertyInfo info) =>
        GetAlternativeNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetRequiredNames(FieldInfo info) =>
        GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetRequiredNames(MethodInfo info) =>
        GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetRequiredNames(PropertyInfo info) =>
        GetRequiredNames(info.Name, info.GetCustomAttributes<CliNameAttribute>());

    public IEnumerable<string> GetRequiredNames(ParameterInfo info) =>
        GetRequiredNames(info.Name!, info.GetCustomAttributes<CliNameAttribute>());

    protected abstract IEnumerable<string> GetAlternativeNames(string name,
        IEnumerable<CliNameAttribute> cliNameAttributes);

    protected abstract IEnumerable<string> GetRequiredNames(string name,
        IEnumerable<CliNameAttribute> cliNameAttributes);
}
