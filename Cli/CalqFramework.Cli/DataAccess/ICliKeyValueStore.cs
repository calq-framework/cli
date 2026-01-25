using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Provides read-write access to a CLI key-value store with accessor grouping for help generation.
    /// </summary>
    public interface ICliKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {

        /// <summary>
        /// Gets keys grouped by their accessors (e.g., fields, properties) for displaying alternative names in help.
        /// </summary>
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}
