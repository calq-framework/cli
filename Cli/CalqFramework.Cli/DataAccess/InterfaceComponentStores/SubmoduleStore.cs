using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores {

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
            return Store.GetAccessorKeysPairs().Select(pair => new Submodule() {
                Keys = pair.Keys,
                MemberInfo = pair.Accessor,
            });
        }

        public object? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }
    }
}
