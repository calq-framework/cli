using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.InterfaceComponents;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliComponentStoreFactory {
        IOptionStore CreateOptionStore(object obj);
        ISubmoduleStore CreateSubmoduleStore(object obj);
        ISubcommandStore CreateSubcommandStore(object obj);
        ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo cliAction, object obj);
    }
}