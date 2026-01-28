using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponents {

    internal class OptionStore : IOptionStore {

        public OptionStore(ICliKeyValueStore<string, string?, MemberInfo> store) {
            Store = store;
        }

        private ICliKeyValueStore<string, string?, MemberInfo> Store { get; }

        public string? this[string key] { get => Store[key]; set => Store[key] = value; }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public IEnumerable<Option> GetOptions() {
            var result = new List<Option>();
            IDictionary<MemberInfo, IEnumerable<string>> dict = Store.GetKeysByAccessors();
            foreach (MemberInfo key in dict.Keys) {
                result.Add(new Option() {
                    Type = GetDataType(dict[key].First()),
                    Keys = dict[key],
                    MemberInfo = key,
                    Value = this[dict[key].First()]
                });
            }
            return result;
        }

        public string? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }
    }
}
