using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliComponentStoreFactory {
        IOptionStore<string, object?, MemberInfo> CreateOptionStore(object obj);
        ISubmoduleStore<string, object?, MemberInfo> CreateSubmoduleStore(object obj);
        MethodResolver CreateMethodResolver(object targetObj);
    }
}