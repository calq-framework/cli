using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    /// <summary>
    /// Provides access to CLI submodules (nested objects) available on an object.
    /// </summary>
    public interface ISubmoduleStore : IKeyValueStore<string, object?> {
        /// <summary>
        /// Gets all available submodules with their metadata for help generation.
        /// </summary>
        IEnumerable<Submodule> GetSubmodules();
    }
}
