using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Provides a function executor for CLI with accessor grouping for help generation.
    /// </summary>
    public interface ICliFunctionExecutor<TKey, TValue, TAccessor> : IFunctionExecutor<TKey, TValue>, ICliKeyValueStore<TKey, TValue, TAccessor> {
        /// <summary>
        /// Sets the parameter values from arguments without applying defaults or validation.
        /// </summary>
        void SetParameterValues();
    }
}
