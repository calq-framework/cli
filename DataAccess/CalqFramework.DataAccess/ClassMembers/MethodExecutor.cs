using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMembers;

/// <summary>
/// Executes methods with parameter assignment and invocation support.
/// </summary>
public class MethodExecutor : MethodExecutorBase<string, object?>, IKeyValueStore<string, object?, ParameterInfo> {

    public MethodExecutor(MethodInfo method, object obj) : base(method, obj) {
    }

    public override bool ContainsAccessor(ParameterInfo accessor) {
        return accessor.Member == ParentMethod;
    }

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
        result = ParameterInfos.FirstOrDefault(x => x.Name == key);
        return result != null;
    }

    protected override object? ConvertFromInternalValue(object? value, ParameterInfo accessor) {
        return value;
    }

    protected override object? ConvertToInternalValue(object? value, ParameterInfo accessor) {
        return value;
    }
}
