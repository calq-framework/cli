using System.Collections.Generic;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores {
    /// <summary>
    /// Executes CLI subcommands (methods) with parameter handling.
    /// </summary>
    public interface ISubcommandExecutor : IFunctionExecutor<string, string?> {
        /// <summary>
        /// Gets all parameters for the subcommand with their metadata for help generation.
        /// </summary>
        IEnumerable<Parameter> GetParameters();

        /// <summary>
        /// Gets the first unassigned parameter, or null if all parameters are assigned.
        /// </summary>
        Parameter? GetFirstUnassignedParameter();
    }
}
