namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class FieldStoreBase<TKey, TValue>(object targetObject, BindingFlags bindingFlags) : ClassDataMemberStoreBase<TKey, TValue, FieldInfo, object?>(targetObject, bindingFlags) {
    public override IEnumerable<FieldInfo> Accessors => TargetType.GetFields(BindingFlags)
        .Where(ContainsAccessor);

    public override object? this[FieldInfo accessor] {
        get => accessor.GetValue(TargetObject);
        set => accessor.SetValue(TargetObject, value);
    }

    public override Type GetValueType(FieldInfo accessor) => accessor.FieldType;

    public override object? GetValueOrInitialize(FieldInfo accessor) {
        object value = accessor.GetValue(TargetObject) ?? accessor.FieldType.CreateInstance();
        accessor.SetValue(TargetObject, value);
        return value;
    }
}
