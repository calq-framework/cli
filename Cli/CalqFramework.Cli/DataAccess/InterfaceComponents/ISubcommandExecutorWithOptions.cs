using System.Collections.Generic;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {
    /// <summary>
    /// Executes CLI subcommands with both parameter and option support (enables shadowing).
    /// </summary>
    public interface ISubcommandExecutorWithOptions : ISubcommandExecutor, IOptionStore {
    }
}
