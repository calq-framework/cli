using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class CliParamStore<TKey, TValue, TAccessor> : ICliParamStore<TKey, TValue, TAccessor> {
        ICliStore<TKey, TValue, TAccessor> Store { get; }

        public TValue this[TKey key] { get => Store[key]; set => Store[key] = value; }

        public CliParamStore(ICliStore<TKey, TValue, TAccessor> store) {
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

        public IEnumerable<PositionalPram> GetParamsString() {
            var result = new List<PositionalPram>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new PositionalPram() {
                    // FIXME generics
                    Type = GetDataType(dict[key].First()),
                    Keys = dict[key].Select(x => x.ToString()),
                    ParamInfo = key as ParameterInfo,
                    Value = this[dict[key].First()]?.ToString(),
                    HasDefaultValue = (key as ParameterInfo).HasDefaultValue
                });
            }
            return result;
        }
    }
}