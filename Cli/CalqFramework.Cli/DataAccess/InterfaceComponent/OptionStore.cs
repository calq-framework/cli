using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class OptionStore<TKey, TValue, TAccessor> : IOptionStore<TKey, TValue, TAccessor> {
        ICliStore<TKey, TValue, TAccessor> Store { get; }

        public TValue this[TKey key] { get => Store[key]; set => Store[key] = value; }

        public OptionStore(ICliStore<TKey, TValue, TAccessor> store) {
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

        public IEnumerable<Option> GetOptions() {
            var result = new List<Option>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Option() {
                    // FIXME generics
                    Type = GetDataType(dict[key].First()),
                    Keys = dict[key].Select(x => x.ToString()),
                    MemberInfo = key as MemberInfo,
                    Value = this[dict[key].First()]?.ToString()
                });
            }
            return result;
        }
    }
}