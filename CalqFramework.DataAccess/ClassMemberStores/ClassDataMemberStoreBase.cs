namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class ClassDataMemberStoreBase<TKey, TValue, TAccessor, TInternalValue>(object targetObject, BindingFlags bindingFlags) : KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> {
    protected BindingFlags BindingFlags { get; } = bindingFlags;
    protected object TargetObject { get; } = targetObject;
    protected Type TargetType { get; } = targetObject.GetType();
}
