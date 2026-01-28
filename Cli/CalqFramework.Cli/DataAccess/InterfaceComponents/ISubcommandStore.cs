using System;
using System.Collections.Generic;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    /// <summary>
    /// Provides read-only access to CLI subcommands (methods) available on an object.
    /// </summary>
    public interface ISubcommandStore : IReadOnlyKeyValueStore<string, MethodInfo?> {

        /// <summary>
        /// Gets all available subcommands with their metadata for help generation.
        /// </summary>
        IEnumerable<Subcommand> GetSubcommands(Func<MethodInfo, object?, ISubcommandExecutor> createSubcommandExecutor);
    }
}
