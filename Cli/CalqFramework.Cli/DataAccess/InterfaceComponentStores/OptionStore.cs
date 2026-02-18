using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores {

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
            return Store.GetAccessorKeysPairs().Select(pair => new Option() {
                ValueType = GetDataType(pair.Keys[0]),
                IsCollection = Store.IsCollection(pair.Keys[0]),
                Keys = pair.Keys,
                MemberInfo = pair.Accessor,
                Value = this[pair.Keys[0]]
            });
        }

        public string? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }
    }
}
