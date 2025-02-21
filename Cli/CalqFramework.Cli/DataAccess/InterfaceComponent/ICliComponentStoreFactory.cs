using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public interface ICliComponentStoreFactory {

        IOptionStore CreateOptionStore(object obj);

        ISubcommandExecutor CreateSubcommandExecutor(MethodInfo method, object? obj);

        ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo method, object obj);

        ISubcommandStore CreateSubcommandStore(object obj);

        ISubmoduleStore CreateSubmoduleStore(object obj);
    }
}
