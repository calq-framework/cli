using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}