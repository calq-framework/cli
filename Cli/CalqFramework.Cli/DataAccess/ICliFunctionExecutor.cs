using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess {

    public interface ICliFunctionExecutor<TKey, TValue, TAccessor> : IFunctionExecutor<TKey, TValue> {

        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}