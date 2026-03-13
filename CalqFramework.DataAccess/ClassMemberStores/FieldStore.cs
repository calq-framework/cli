namespace CalqFramework.DataAccess.ClassMemberStores;

/// <summary>
///     Provides key-value access to object fields by name.
/// </summary>
public sealed class FieldStore(object obj, BindingFlags bindingFlags) : FieldStoreBase<string, object?>(obj, bindingFlags), IKeyValueStore<string, object?, FieldInfo> {
    public override bool ContainsAccessor(FieldInfo accessor) => accessor.ReflectedType == TargetType;

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
        result = TargetType.GetField(key, BindingFlags);
        return result != null;
    }

    protected override object? ConvertFromInternalValue(object? value, FieldInfo accessor) => value;

    protected override object? ConvertToInternalValue(object? value, FieldInfo accessor) => value;
}
