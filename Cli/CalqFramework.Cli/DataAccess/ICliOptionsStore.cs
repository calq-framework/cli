using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliOptionsStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IEnumerable<Option> GetOptionsString();
    }
}