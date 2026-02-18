using System.Reflection;

namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class ClassDataMemberStoreBase<TKey, TValue, TAccessor, TInternalValue> : KeyValueStoreBase<TKey, TValue, TAccessor, TInternalValue> {

    protected ClassDataMemberStoreBase(object targetObject, BindingFlags bindingFlags) {
        TargetObject = targetObject;
        BindingFlags = bindingFlags;
        TargetType = targetObject.GetType();
    }

    protected BindingFlags BindingFlags { get; }
    protected object TargetObject { get; }
    protected Type TargetType { get; }
}
