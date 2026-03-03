using System.Collections.Generic;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores {
    /// <summary>
    /// Provides access to CLI options (fields and properties) with string values.
    /// </summary>
    public interface IOptionStore : IKeyValueStore<string, string?> {
        /// <summary>
        /// Gets all available options with their metadata for help generation.
        /// </summary>
        IEnumerable<Option> GetOptions();
    }
}
