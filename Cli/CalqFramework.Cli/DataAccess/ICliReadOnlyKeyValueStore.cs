using CalqFramework.DataAccess;
using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliReadOnlyKeyValueStore<TKey, TValue, TAccessor> : IReadOnlyKeyValueStore<TKey, TValue> {
        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}