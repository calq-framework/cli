using System.Reflection;

namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class FieldStoreBase<TKey, TValue> : ClassDataMemberStoreBase<TKey, TValue, FieldInfo, object?> {

    public FieldStoreBase(object targetObject, BindingFlags bindingFlags) : base(targetObject, bindingFlags) {
    }

    public override IEnumerable<FieldInfo> Accessors => TargetType.GetFields(BindingFlags).Where(ContainsAccessor);

    public override object? this[FieldInfo accessor] {
        get {
            return accessor.GetValue(TargetObject);
        }
        set {
            accessor.SetValue(TargetObject, value);
        }
    }

    public override Type GetDataType(FieldInfo accessor) {
        return accessor.FieldType;
    }

    public override object? GetValueOrInitialize(FieldInfo accessor) {
        object value = accessor.GetValue(TargetObject) ??
               Activator.CreateInstance(accessor.FieldType) ??
               Activator.CreateInstance(Nullable.GetUnderlyingType(accessor.FieldType)!)!;
        accessor.SetValue(TargetObject, value);
        return value;
    }
}
