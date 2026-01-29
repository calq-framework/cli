using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Provides read-write access to a CLI key-value store with accessor grouping for help generation.
    /// </summary>
    public interface ICliKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {

        /// <summary>
        /// Gets an ordered list of accessor-to-keys pairs for displaying alternative names in help.
        /// </summary>
        IEnumerable<AccessorKeysPair<TAccessor>> GetAccessorKeysPairs();
    }
}
