using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class ParameterStore : IParameterStore {
        ICliStore<string, string?, ParameterInfo> Store { get; }

        public string? this[string key] { get => Store[key]; set => Store[key] = value; }

        public ParameterStore(ICliStore<string, string?, ParameterInfo> store) {
            Store = store;
        }

        public bool ContainsKey(string key) {
            return Store.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Store.GetDataType(key);
        }

        public string? GetValueOrInitialize(string key) {
            return Store.GetValueOrInitialize(key);
        }

        public IEnumerable<Parameter> GetParameters() {
            var result = new List<Parameter>();
            var dict = Store.GetKeysByAccessors();
            foreach (var key in dict.Keys) {
                result.Add(new Parameter() {
                    Type = GetDataType(dict[key].First()),
                    Keys = dict[key],
                    ParameterInfo = key,
                    Value = this[dict[key].First()],
                    HasDefaultValue = key.HasDefaultValue
                });
            }
            return result;
        }
    }
}