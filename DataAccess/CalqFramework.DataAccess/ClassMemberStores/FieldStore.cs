using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMemberStores;

/// <summary>
/// Provides key-value access to object fields by name.
/// </summary>
public sealed class FieldStore : FieldStoreBase<string, object?>, IKeyValueStore<string, object?, FieldInfo> {

    public FieldStore(object obj, BindingFlags bindingFlags) : base(obj, bindingFlags) {
    }

    public override bool ContainsAccessor(FieldInfo accessor) {
        return accessor.ReflectedType == ParentType;
    }

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
        result = ParentType.GetField(key, BindingFlags);
        return result != null;
    }

    protected override object? ConvertFromInternalValue(object? value, FieldInfo accessor) {
        return value;
    }

    protected override object? ConvertToInternalValue(object? value, FieldInfo accessor) {
        return value;
    }
}
