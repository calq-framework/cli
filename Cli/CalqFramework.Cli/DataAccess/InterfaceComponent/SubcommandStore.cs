using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class SubcommandStore : ISubcommandStore {
        IReadOnlyClassMemberStore<string, MethodInfo?, MethodInfo> Store { get; }

        public MethodInfo? this[string key] { get => Store[key]; }

        public SubcommandStore(IReadOnlyClassMemberStore<string, MethodInfo?, MethodInfo> store) {
            Store = store;
        }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public IEnumerable<Subcommand> GetSubcommands() {
            var result = new List<Subcommand>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Subcommand() {
                    ReturnType = GetDataType(dict[key].First()),
                    Keys = dict[key],
                    MethodInfo = key
                });
            }
            return result;
        }
    }
}