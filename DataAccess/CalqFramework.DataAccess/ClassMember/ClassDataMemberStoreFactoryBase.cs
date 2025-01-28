using CalqFramework.DataAccess;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember;
public abstract class ClassDataMemberStoreFactoryBase<TKey, TValue> : IClassDataMemberStoreFactory<TKey, TValue> {

    public const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    public bool AccessFields { get; init; } = false;
    public bool AccessProperties { get; init; } = true;

    public BindingFlags BindingAttr { get; init; } = DefaultLookup;

    public virtual IKeyValueStore<TKey, TValue, MemberInfo> CreateDataMemberStore(object obj) {
        if (AccessFields && AccessProperties) {
            return CreateFieldAndPropertyStore(obj);
        } else if (AccessFields) {
            return CreateFieldStore(obj);
        } else if (AccessProperties) {
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
