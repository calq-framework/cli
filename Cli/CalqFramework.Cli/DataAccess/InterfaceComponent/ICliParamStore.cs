using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliParamStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IEnumerable<PositionalPram> GetParamsString();
    }
}