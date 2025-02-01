using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliOptionsStoreFactory {
        bool AccessFields { get; init; }
        bool AccessProperties { get; init; }
        BindingFlags BindingAttr { get; init; }

        ICliCommandStore<string, object?, MemberInfo> CreateCommandStore(object obj);
        ICliOptionsStore<string, object?, MemberInfo> CreateOptonStore(object obj);
    }
}