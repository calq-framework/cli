using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliKeyValueStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}