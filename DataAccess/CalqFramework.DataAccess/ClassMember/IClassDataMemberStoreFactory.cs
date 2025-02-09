using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public interface IClassDataMemberStoreFactory {
        IKeyValueStore<string, object?, MemberInfo> CreateDataMemberStore(object obj);
    }
}