using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    public interface ICliComponentStoreFactory {

        IOptionStore CreateOptionStore(object obj);

        ISubcommandExecutor CreateSubcommandExecutor(MethodInfo cliAction, object? obj);

        ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo cliAction, object obj);

        ISubcommandStore CreateSubcommandStore(object obj);

        ISubmoduleStore CreateSubmoduleStore(object obj);
    }
}
