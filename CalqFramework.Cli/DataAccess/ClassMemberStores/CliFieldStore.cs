using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMemberStores;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores;

internal class CliFieldStore<TValue> : FieldStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {
    public CliFieldStore(object targetObject, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier,
        IAccessValidator accessValidator, ICompositeValueConverter<TValue> compositeValueConverter) : base(targetObject,
        bindingFlags) {
        ClassMemberStringifier = classMemberStringifier;
        AccessValidator = accessValidator;
        CompositeValueConverter = compositeValueConverter;
        AccessorsByNames = GetAccessorsByNames();
    }

    private IDictionary<string, FieldInfo> AccessorsByNames { get; }
    private IAccessValidator AccessValidator { get; }
    private IClassMemberStringifier ClassMemberStringifier { get; }
    private ICompositeValueConverter<TValue> CompositeValueConverter { get; }

    public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() =>
        AccessorsByNames
            .GroupBy(kv => kv.Value)
            .Select(g => new AccessorKeysPair<MemberInfo>(
                g.Key,
                g.Select(kv => kv.Key).ToArray()
            ));

    public bool IsMultiValue(string key) {
        FieldInfo accessor = GetAccessor(key);
        return CompositeValueConverter.IsMultiValue(accessor.FieldType);
    }

    public override bool ContainsAccessor(FieldInfo accessor) =>
        accessor.ReflectedType == TargetType && AccessValidator.IsValid(accessor);

    public override Type GetValueType(FieldInfo accessor) => CompositeValueConverter.GetValueType(accessor.FieldType);

    public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) =>
        AccessorsByNames.TryGetValue(key, out result);

    protected override TValue ConvertFromInternalValue(object? value, FieldInfo accessor) =>
        CompositeValueConverter.ConvertFrom(value, accessor.FieldType);

    protected override object? ConvertToInternalValue(TValue value, FieldInfo accessor) {
        object? currentValue = GetValueOrInitialize(accessor);
        return CompositeValueConverter.ConvertToOrUpdate(value, accessor.FieldType, currentValue);
    }

    private IDictionary<string, FieldInfo> GetAccessorsByNames() {
        StringComparer stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        Dictionary<string, FieldInfo> accessorsByRequiredNames = new(stringComparer);
        foreach (FieldInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                    throw CliErrors.NameCollision(accessor.Name, accessorsByRequiredNames[name].Name);
                }
            }
        }

        Dictionary<string, FieldInfo> accessorsByAlternativeNames = new(stringComparer);
        HashSet<string> collidingAlternativeNames = [];
        foreach (FieldInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                    collidingAlternativeNames.Add(name);
                }
            }
        }

        foreach (string name in collidingAlternativeNames) {
            accessorsByAlternativeNames.Remove(name);
        }

        Dictionary<string, FieldInfo> accessorsByNames = accessorsByRequiredNames;
        foreach (string name in accessorsByAlternativeNames.Keys) {
            accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
        }

        return accessorsByNames;
    }
}
