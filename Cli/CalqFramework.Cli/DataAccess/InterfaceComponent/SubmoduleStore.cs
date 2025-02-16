using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class SubmoduleStore : ISubmoduleStore {

        public SubmoduleStore(ICliKeyValueStore<string, object?, MemberInfo> store) {
            Store = store;
        }

        private ICliKeyValueStore<string, object?, MemberInfo> Store { get; }

        public object? this[string key] { get => Store[key]; set => Store[key] = value; }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public IEnumerable<Submodule> GetSubmodules() {
            var result = new List<Submodule>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Submodule() {
                    Keys = dict[key],
                    MemberInfo = key,
                });
            }
            return result;
        }

        public object? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }
    }
}