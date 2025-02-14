using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class SubcommandExecutor : ISubcommandExecutor {
        ICliFunctionExecutor<string, string?, ParameterInfo> Executor { get; }

        public string? this[string key] { get => Executor[key]; set => Executor[key] = value; }

        public SubcommandExecutor(ICliFunctionExecutor<string, string?, ParameterInfo> store) {
            Executor = store;
        }

        public bool ContainsKey(string key) {
            return Executor.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Executor.GetDataType(key);
        }

        public string? GetValueOrInitialize(string key) {
            return Executor.GetValueOrInitialize(key);
        }

        public IEnumerable<Parameter> GetParameters() {
            var result = new List<Parameter>();
            var dict = Executor.GetKeysByAccessors();
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

        public void AddParameter(string? value) {
            Executor.AddParameter(value);
        }

        public object? Invoke() {
            return Executor.Invoke();
        }
    }
}