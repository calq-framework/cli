using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class SubmoduleStore<TKey, TValue, TAccessor> : ISubmoduleStore<TKey, TValue, TAccessor> {
        ICliStore<TKey, TValue, TAccessor> Store { get; }

        public TValue this[TKey key] { get => Store[key]; set => Store[key] = value; }

        public SubmoduleStore(ICliStore<TKey, TValue, TAccessor> store) {
            Store = store;
        }

        public bool ContainsKey(TKey key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(TKey key) {
            return Store.GetDataType(key);
        }

        public TValue GetValueOrInitialize(TKey key) {
            return Store.GetValueOrInitialize(key);
        }

        public IEnumerable<Submodule> GetCommands() {
            var result = new List<Submodule>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Submodule() {
                    // FIXME generics
                    Keys = dict[key].Select(x => x.ToString()),
                    MemberInfo = key as MemberInfo,
                });
            }
            return result;
        }
    }
}