using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    public interface ICliFunctionExecutor<TKey, TValue, TAccessor> : IFunctionExecutor<TKey, TValue> {

        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}
