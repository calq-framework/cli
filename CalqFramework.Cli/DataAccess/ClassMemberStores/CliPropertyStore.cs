using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMemberStores;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores;

internal class CliPropertyStore<TValue> : PropertyStoreBase<string, TValue>,
    ICliKeyValueStore<string, TValue, MemberInfo> {
    public CliPropertyStore(object targetObject, BindingFlags bindingFlags,
        IClassMemberStringifier classMemberStringifier, IAccessValidator accessValidator,
        ICompositeValueConverter<TValue> compositeValueConverter) : base(targetObject, bindingFlags) {
        ClassMemberStringifier = classMemberStringifier;
        AccessValidator = accessValidator;
        CompositeValueConverter = compositeValueConverter;
        AccessorsByNames = GetAccessorsByNames();
    }

    protected IClassMemberStringifier ClassMemberStringifier { get; }
    private IDictionary<string, PropertyInfo> AccessorsByNames { get; }
    private IAccessValidator AccessValidator { get; }
    private ICompositeValueConverter<TValue> CompositeValueConverter { get; }

    public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() =>
        AccessorsByNames
            .GroupBy(kv => kv.Value)
            .Select(g => new AccessorKeysPair<MemberInfo>(
                g.Key,
                g.Select(kv => kv.Key).ToArray()
            ));

    public bool IsMultiValue(string key) {
        PropertyInfo accessor = GetAccessor(key);
        return CompositeValueConverter.IsMultiValue(accessor.PropertyType);
    }

    public override bool ContainsAccessor(PropertyInfo accessor) =>
        accessor.ReflectedType == TargetType && AccessValidator.IsValid(accessor);

    public override Type GetValueType(PropertyInfo accessor) =>
        CompositeValueConverter.GetValueType(accessor.PropertyType);

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) =>
        AccessorsByNames.TryGetValue(key, out result);

    protected override TValue ConvertFromInternalValue(object? value, PropertyInfo accessor) =>
        CompositeValueConverter.ConvertFrom(value, accessor.PropertyType);

    protected override object? ConvertToInternalValue(TValue value, PropertyInfo accessor) {
        object? currentValue = GetValueOrInitialize(accessor);
        return CompositeValueConverter.ConvertToOrUpdate(value, accessor.PropertyType, currentValue);
    }

    private IDictionary<string, PropertyInfo> GetAccessorsByNames() {
        StringComparer stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        Dictionary<string, PropertyInfo> accessorsByRequiredNames = new(stringComparer);
        foreach (PropertyInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                    throw CliErrors.NameCollision(accessor.Name, accessorsByRequiredNames[name].Name);
                }
            }
        }

        Dictionary<string, PropertyInfo> accessorsByAlternativeNames = new(stringComparer);
        HashSet<string> collidingAlternativeNames = new();
        foreach (PropertyInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                    collidingAlternativeNames.Add(name);
                }
            }
        }

        foreach (string name in collidingAlternativeNames) {
            accessorsByAlternativeNames.Remove(name);
        }

        Dictionary<string, PropertyInfo> accessorsByNames = accessorsByRequiredNames;
        foreach (string name in accessorsByAlternativeNames.Keys) {
            accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
        }

        return accessorsByNames;
    }
}
