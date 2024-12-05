using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember;
public abstract class ClassDataMemberStoreFactoryBase<TKey, TValue> {

    public ClassDataMemberStoreFactoryOptions ClassDataMemberStoreFactoryOptions { get; }

    public ClassDataMemberStoreFactoryBase(ClassDataMemberStoreFactoryOptions classDataMemberAccessorOptions) {
        ClassDataMemberStoreFactoryOptions = classDataMemberAccessorOptions;
    }

    public virtual IKeyValueStore<TKey, TValue, MemberInfo> CreateDataMemberStore(object obj) {
        if (ClassDataMemberStoreFactoryOptions.AccessFields && ClassDataMemberStoreFactoryOptions.AccessProperties) {
            return CreateFieldAndPropertyStore(obj);
        } else if (ClassDataMemberStoreFactoryOptions.AccessFields) {
            return CreateFieldStore(obj);
        } else if (ClassDataMemberStoreFactoryOptions.AccessProperties) {
            return CreatePropertyStore(obj);
        } else {
            throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
        }
    }

    protected virtual IKeyValueStore<TKey, TValue, MemberInfo> CreateFieldAndPropertyStore(object obj) {
        return new DualKeyValueStore<TKey, TValue, MemberInfo>(CreateFieldStore(obj), CreatePropertyStore(obj));
    }
    protected abstract IKeyValueStore<TKey, TValue, MemberInfo> CreateFieldStore(object obj);
    protected abstract IKeyValueStore<TKey, TValue, MemberInfo> CreatePropertyStore(object obj);
}
