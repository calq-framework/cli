using System.Reflection;
using CalqFramework.DataAccess.Extensions.System;

namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class PropertyStoreBase<TKey, TValue>(object targetObject, BindingFlags bindingFlags) : ClassDataMemberStoreBase<TKey, TValue, PropertyInfo, object?>(targetObject, bindingFlags) {
    public override IEnumerable<PropertyInfo> Accessors =>
        TargetType.GetProperties(BindingFlags).Where(ContainsAccessor);

    public override object? this[PropertyInfo accessor] {
        get => accessor.GetValue(TargetObject);
        set => accessor.SetValue(TargetObject, value);
    }

    public override Type GetValueType(PropertyInfo accessor) => accessor.PropertyType;

    public override object? GetValueOrInitialize(PropertyInfo accessor) {
        object value = accessor.GetValue(TargetObject) ??
                       accessor.PropertyType.CreateInstance();
        accessor.SetValue(TargetObject, value);
        return value;
    }
}
