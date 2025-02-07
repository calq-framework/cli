using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface ICliCommandStore<TKey, TValue, TAccessor> : IKeyValueStore<TKey, TValue> {
        IEnumerable<Command> GetCommandsString();
    }
}