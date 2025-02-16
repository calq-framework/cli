using System.Collections.Generic;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess {

    public interface ICliReadOnlyKeyValueStore<TKey, TValue, TAccessor> : IReadOnlyKeyValueStore<TKey, TValue> {

        IDictionary<TAccessor, IEnumerable<TKey>> GetKeysByAccessors();
    }
}
