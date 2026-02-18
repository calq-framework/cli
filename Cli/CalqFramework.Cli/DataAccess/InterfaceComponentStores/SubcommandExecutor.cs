using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores {

    internal class SubcommandExecutor : ISubcommandExecutor {

        public SubcommandExecutor(ICliFunctionExecutor<string, string?, ParameterInfo> executor) {
            Executor = executor;
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
            return Executor.GetAccessorKeysPairs().Select(pair => new Parameter() {
                ValueType = GetDataType(pair.Keys[0]),
                IsCollection = Executor.IsCollection(pair.Keys[0]),
                Keys = pair.Keys,
                ParameterInfo = pair.Accessor,
                Value = this[pair.Keys[0]],
                HasDefaultValue = pair.Accessor.HasDefaultValue
            });
        }

        public Parameter? GetFirstUnassignedParameter() {
            Executor.SetParameterValues();
            return GetParameters().FirstOrDefault(p => Equals(p.Value, null));
        }

        public string? GetValueOrInitialize(string key) {
            return Executor.GetValueOrInitialize(key);
        }

        public object? Invoke() {
            return Executor.Invoke();
        }
    }
}
