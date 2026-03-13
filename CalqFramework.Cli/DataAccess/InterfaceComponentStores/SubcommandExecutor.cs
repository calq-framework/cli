using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

internal sealed class SubcommandExecutor(ICliFunctionExecutor<string, string?, ParameterInfo> executor) : ISubcommandExecutor {
    private ICliFunctionExecutor<string, string?, ParameterInfo> Executor { get; } = executor;

    public string? this[string key] {
        get => Executor[key];
        set => Executor[key] = value;
    }

    public void AddArgument(string? value) => Executor.AddArgument(value);

    public bool ContainsKey(string key) => Executor.ContainsKey(key);

    public Type GetValueType(string key) => Executor.GetValueType(key);

    public IEnumerable<Parameter> GetParameters() =>
        Executor.GetAccessorKeysPairs()
            .Select(pair => new Parameter {
                ValueType = GetValueType(pair.Keys[0]),
                IsMultiValue = Executor.IsMultiValue(pair.Keys[0]),
                Keys = pair.Keys,
                ParameterInfo = pair.Accessor,
                Value = this[pair.Keys[0]],
                HasDefaultValue = pair.Accessor.HasDefaultValue
            });

    public Parameter? GetFirstUnassignedParameter() {
        Executor.SetParameterValues();
        return GetParameters()
            .FirstOrDefault(p => Equals(p.Value, null));
    }

    public string? GetValueOrInitialize(string key) => Executor.GetValueOrInitialize(key);

    public object? Invoke() => Executor.Invoke();
}
