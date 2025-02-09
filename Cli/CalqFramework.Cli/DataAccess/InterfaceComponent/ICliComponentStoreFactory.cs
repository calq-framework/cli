using CalqFramework.Cli.DataAccess.ClassMember;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliComponentStoreFactory {
        IOptionStore CreateOptionStore(object obj);
        ISubmoduleStore CreateSubmoduleStore(object obj);
        MethodResolver CreateMethodResolver(object targetObj);
    }
}