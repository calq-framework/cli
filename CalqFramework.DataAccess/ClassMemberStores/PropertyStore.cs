namespace CalqFramework.DataAccess.ClassMemberStores;

/// <summary>
///     Provides key-value access to object properties by name.
/// </summary>
public sealed class PropertyStore(object obj, BindingFlags bindingFlags) : PropertyStoreBase<string, object?>(obj, bindingFlags), IKeyValueStore<string, object?, PropertyInfo> {
    public override bool ContainsAccessor(PropertyInfo accessor) => accessor.ReflectedType == TargetType;

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
        result = TargetType.GetProperty(key, BindingFlags);
        return result != null;
    }

    protected override object? ConvertFromInternalValue(object? value, PropertyInfo accessor) => value;

    protected override object? ConvertToInternalValue(object? value, PropertyInfo accessor) => value;
}
