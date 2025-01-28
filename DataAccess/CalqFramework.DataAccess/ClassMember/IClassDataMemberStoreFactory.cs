using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public interface IClassDataMemberStoreFactory<TKey, TValue> {
        bool AccessFields { get; init; }
        bool AccessProperties { get; init; }
        BindingFlags BindingAttr { get; init; }

        IKeyValueStore<TKey, TValue, MemberInfo> CreateDataMemberStore(object obj);
    }
}