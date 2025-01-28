using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliOptionsStoreFactory : IClassDataMemberStoreFactory<string, object?> {
        ICliOptionsStore CreateCliStore(object obj);
    }
}