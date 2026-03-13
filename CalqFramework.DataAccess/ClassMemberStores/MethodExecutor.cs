namespace CalqFramework.DataAccess.ClassMemberStores;

/// <summary>
///     Executes methods with parameter assignment and invocation support.
/// </summary>
public class MethodExecutor(MethodInfo method, object obj) : MethodExecutorBase<string, object?>(method, obj), IKeyValueStore<string, object?, ParameterInfo> {
    public override bool ContainsAccessor(ParameterInfo accessor) => accessor.Member == ParentMethod;

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
        result = ParameterInfos.FirstOrDefault(x => x.Name == key);
        return result != null;
    }

    protected override object? ConvertFromInternalValue(object? value, ParameterInfo accessor) => value;

    protected override object? ConvertToInternalValue(object? value, ParameterInfo accessor) => value;
}
