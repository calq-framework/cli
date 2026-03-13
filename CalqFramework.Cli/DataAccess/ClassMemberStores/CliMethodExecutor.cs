using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMemberStores;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores;

internal sealed class CliMethodExecutor<TValue> : MethodExecutorBase<string, TValue>, ICliFunctionExecutor<string, TValue, ParameterInfo> {
    public CliMethodExecutor(MethodInfo method, object? obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, ICompositeValueConverter<TValue> compositeValueConverter) : base(method, obj) {
        BindingFlags = bindingFlags;
        ClassMemberStringifier = classMemberStringifier;
        CompositeValueConverter = compositeValueConverter;
        AccessorsByNames = GetAccessorsByNames();
    }

    private BindingFlags BindingFlags { get; }
    private IClassMemberStringifier ClassMemberStringifier { get; }
    private Dictionary<string, ParameterInfo> AccessorsByNames { get; }
    private ICompositeValueConverter<TValue> CompositeValueConverter { get; }

    public IEnumerable<AccessorKeysPair<ParameterInfo>> GetAccessorKeysPairs() =>
        AccessorsByNames.GroupBy(kv => kv.Value)
            .OrderBy(g => ((ParameterInfo)g.Key).Position)
            .Select(g => new AccessorKeysPair<ParameterInfo>(
                (ParameterInfo)g.Key,
                g.Select(kv => kv.Key)
                    .ToArray()));

    public bool IsMultiValue(string key) {
        ParameterInfo accessor = GetAccessor(key);
        return CompositeValueConverter.IsMultiValue(accessor.ParameterType);
    }

    public void SetParameterValues() {
        bool IsAssigned(int i) => ParameterValues[i] != DBNull.Value;

        int argumentIndex = 0;
        for (int parameterIndex = 0; parameterIndex < ParameterValues.Length; ++parameterIndex) {
            if (!IsAssigned(parameterIndex)) {
                if (argumentIndex < Arguments.Count) {
                    object? value = ConvertToInternalValue(Arguments[argumentIndex++], ParameterInfos[parameterIndex]);
                    ParameterValues[parameterIndex] = value;
                }
            }
        }

        if (argumentIndex < Arguments.Count) {
            throw DataAccessErrors.UnexpectedArgument(Arguments[argumentIndex]);
        }
    }

    public override bool ContainsAccessor(ParameterInfo accessor) => accessor.Member == ParentMethod;

    public override Type GetValueType(ParameterInfo accessor) =>
        CompositeValueConverter.GetValueType(accessor.ParameterType);

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) =>
        AccessorsByNames.TryGetValue(key, out result);

    protected override TValue ConvertFromInternalValue(object? value, ParameterInfo accessor) =>
        CompositeValueConverter.ConvertFrom(value, accessor.ParameterType);

    protected override object? ConvertToInternalValue(TValue value, ParameterInfo accessor) {
        object? currentValue = GetValueOrInitialize(accessor);
        return CompositeValueConverter.ConvertToOrUpdate(value, accessor.ParameterType, currentValue);
    }

    private Dictionary<string, ParameterInfo> GetAccessorsByNames() {
        StringComparer stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        Dictionary<string, ParameterInfo> accessorsByRequiredNames = new(stringComparer);
        foreach (ParameterInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                    throw CliErrors.NameCollision(accessor.Name!, accessorsByRequiredNames[name].Name!);
                }
            }
        }

        Dictionary<string, ParameterInfo> accessorsByAlternativeNames = new(stringComparer);
        HashSet<string> collidingAlternativeNames = [];
        foreach (ParameterInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                    collidingAlternativeNames.Add(name);
                }
            }
        }

        foreach (string name in collidingAlternativeNames) {
            accessorsByAlternativeNames.Remove(name);
        }

        Dictionary<string, ParameterInfo> accessorsByNames = accessorsByRequiredNames;
        foreach (string name in accessorsByAlternativeNames.Keys) {
            accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
        }

        return accessorsByNames;
    }
}
