using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Provides a function executor for CLI with accessor grouping for help generation.
    /// </summary>
    public interface ICliFunctionExecutor<TKey, TValue, TAccessor> : IFunctionExecutor<TKey, TValue> {

        /// <summary>
        /// Gets keys grouped by their accessors (e.g., parameters) for displaying alternative names in help.
        /// </summary>
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}
