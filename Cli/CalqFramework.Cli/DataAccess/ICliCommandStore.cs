using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    public interface ICliCommandStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IEnumerable<Command> GetCommandsString();
    }
}