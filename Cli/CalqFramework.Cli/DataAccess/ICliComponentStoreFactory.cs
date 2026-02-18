using System.Reflection;
using CalqFramework.Cli.DataAccess.InterfaceComponentStores;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Factory for creating CLI component stores (options, subcommands, submodules) from class objects.
    /// </summary>
    public interface ICliComponentStoreFactory {

        /// <summary>
        /// Creates a store for CLI options (fields and properties) from the specified object.
        /// </summary>
        IOptionStore CreateOptionStore(object obj);

        /// <summary>
        /// Creates an executor for a CLI subcommand (method) without options.
        /// </summary>
        ISubcommandExecutor CreateSubcommandExecutor(MethodInfo method, object? obj);

        /// <summary>
        /// Creates an executor for a CLI subcommand (method) with options support.
        /// </summary>
        ISubcommandExecutorWithOptions CreateSubcommandExecutorWithOptions(MethodInfo method, object obj);

        /// <summary>
        /// Creates a store for CLI subcommands (methods) from the specified object.
        /// </summary>
        ISubcommandStore CreateSubcommandStore(object obj);

        /// <summary>
        /// Creates a store for CLI submodules (nested objects) from the specified object.
        /// </summary>
        ISubmoduleStore CreateSubmoduleStore(object obj);
    }
}
