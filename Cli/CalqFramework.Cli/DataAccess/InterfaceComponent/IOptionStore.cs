using System.Collections.Generic;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    public interface IOptionStore : IKeyValueStore<string, string?> {
        IEnumerable<Option> GetOptions();
    }
}
