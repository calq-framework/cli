using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class SubcommandStore : ISubcommandStore {

        public SubcommandStore(ICliReadOnlyKeyValueStore<string, MethodInfo?, MethodInfo> store) {
            Store = store;
        }

        private ICliReadOnlyKeyValueStore<string, MethodInfo?, MethodInfo> Store { get; }

        public MethodInfo? this[string key] { get => Store[key]; }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public IEnumerable<Subcommand> GetSubcommands(Func<MethodInfo, object?, ISubcommandExecutor> createSubcommandExecutor) {
            var result = new List<Subcommand>();
            IDictionary<MethodInfo, IEnumerable<string>> dict = Store.GetKeysByAccessors();
            foreach (MethodInfo key in dict.Keys) {
                result.Add(new Subcommand() {
                    ReturnType = GetDataType(dict[key].First()),
                    Keys = dict[key],
                    MethodInfo = key,
                    Parameters = createSubcommandExecutor(key, null).GetParameters()
                });
            }
            return result;
        }
    }
}
