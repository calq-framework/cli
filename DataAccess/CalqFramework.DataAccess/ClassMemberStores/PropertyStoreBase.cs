using System.Reflection;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class PropertyStoreBase<TKey, TValue> : ClassDataMemberStoreBase<TKey, TValue, PropertyInfo, object?> {

    public PropertyStoreBase(object targetObject, BindingFlags bindingFlags) : base(targetObject, bindingFlags) {
    }

    public override IEnumerable<PropertyInfo> Accessors => TargetType.GetProperties(BindingFlags).Where(ContainsAccessor);

    public override object? this[PropertyInfo accessor] {
        get {
            return accessor.GetValue(TargetObject);
        }
        set {
            accessor.SetValue(TargetObject, value);
        }
    }

    public override Type GetValueType(PropertyInfo accessor) {
        return accessor.PropertyType;
    }

    public override object? GetValueOrInitialize(PropertyInfo accessor) {
        object value = accessor.GetValue(TargetObject) ??
               accessor.PropertyType.CreateInstance();
        accessor.SetValue(TargetObject, value);
        return value;
    }
}
