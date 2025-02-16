using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {

    internal class SubcommandExecutor : ISubcommandExecutor {

        public SubcommandExecutor(ICliFunctionExecutor<string, string?, ParameterInfo> store) {
            Executor = store;
        }

        private ICliFunctionExecutor<string, string?, ParameterInfo> Executor { get; }

        public string? this[string key] { get => Executor[key]; set => Executor[key] = value; }

        public void AddArgument(string? value) {
            Executor.AddArgument(value);
        }

        public bool ContainsKey(string key) {
            return Executor.ContainsKey(key);
        }

        public Type GetDataType(string key) {
            return Executor.GetDataType(key);
        }

        public IEnumerable<Parameter> GetParameters() {
            var result = new List<Parameter>();
            IDictionary<ParameterInfo, IEnumerable<string>> dict = Executor.GetKeysByAccessors();
            foreach (ParameterInfo key in dict.Keys) {
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

        public string? GetValueOrInitialize(string key) {
            return Executor.GetValueOrInitialize(key);
        }

        public object? Invoke() {
            return Executor.Invoke();
        }
    }
}
