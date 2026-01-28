using System.Reflection;

namespace CalqFramework.DataAccess.ClassMembers;

public abstract class FieldStoreBase<TKey, TValue> : ClassDataMemberStoreBase<TKey, TValue, FieldInfo, object?> {

    public FieldStoreBase(object obj, BindingFlags bindingFlags) : base(obj, bindingFlags) {
    }

    public override IEnumerable<FieldInfo> Accessors => ParentType.GetFields(BindingFlags).Where(ContainsAccessor);

    public override object? this[FieldInfo accessor] {
        get {
            return ((FieldInfo)accessor).GetValue(ParentObject);
        }
        set {
            ((FieldInfo)accessor).SetValue(ParentObject, value);
        }
    }

    public override Type GetDataType(FieldInfo accessor) {
        return ((FieldInfo)accessor).FieldType;
    }

    public override object? GetValueOrInitialize(FieldInfo accessor) {
        object value = ((FieldInfo)accessor).GetValue(ParentObject) ??
               Activator.CreateInstance(((FieldInfo)accessor).FieldType) ??
               Activator.CreateInstance(Nullable.GetUnderlyingType(((FieldInfo)accessor).FieldType)!)!;
        ((FieldInfo)accessor).SetValue(ParentObject, value);
        return value;
    }
}
