using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Formatting;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores;

internal class MethodInfoStore : ICliReadOnlyKeyValueStore<string, MethodInfo, MethodInfo> {
    public MethodInfoStore(object targetObject, BindingFlags bindingFlags,
        IClassMemberStringifier classMemberStringifier, IAccessValidator accessValidator) {
        TargetObject = targetObject;
        BindingFlags = bindingFlags;
        ClassMemberStringifier = classMemberStringifier;
        AccessValidator = accessValidator;
        TargetType = targetObject.GetType();
        AccessorsByNames = GetAccessorsByNames();
    }

    public IEnumerable<MethodInfo> Accessors => TargetType.GetMethods(BindingFlags).Where(ContainsAccessor);
    public IAccessValidator AccessValidator { get; }
    protected BindingFlags BindingFlags { get; }
    protected IClassMemberStringifier ClassMemberStringifier { get; }
    protected object TargetObject { get; }
    protected Type TargetType { get; }
    private Dictionary<string, MethodInfo> AccessorsByNames { get; }

    public MethodInfo this[string key] {
        get {
            if (!TryGetAccessor(key, out MethodInfo? result)) {
                throw CliErrors.InvalidSubcommand(key);
            }

            return result;
        }
    }

    public bool ContainsKey(string key) => TryGetAccessor(key, out _);

    public Type GetValueType(string key) => this[key]!.ReturnType;

    public IEnumerable<AccessorKeysPair<MethodInfo>> GetAccessorKeysPairs() =>
        AccessorsByNames
            .GroupBy(kv => kv.Value)
            .Select(g => new AccessorKeysPair<MethodInfo>(
                g.Key,
                g.Select(kv => kv.Key).ToArray()
            ));

    public bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MethodInfo result) =>
        AccessorsByNames.TryGetValue(key, out result);

    private bool ContainsAccessor(MethodInfo accessor) =>
        accessor.ReflectedType == TargetType && AccessValidator.IsValid(accessor);

    private Dictionary<string, MethodInfo> GetAccessorsByNames() {
        StringComparer stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        Dictionary<string, MethodInfo> accessorsByRequiredNames = new(stringComparer);
        foreach (MethodInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                    throw CliErrors.NameCollision(accessor.Name, accessorsByRequiredNames[name].Name);
                }
            }
        }

        Dictionary<string, MethodInfo> accessorsByAlternativeNames = new(stringComparer);
        HashSet<string> collidingAlternativeNames = [];
        foreach (MethodInfo accessor in Accessors) {
            foreach (string name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                    collidingAlternativeNames.Add(name);
                }
            }
        }

        foreach (string name in collidingAlternativeNames) {
            accessorsByAlternativeNames.Remove(name);
        }

        Dictionary<string, MethodInfo> accessorsByNames = accessorsByRequiredNames;
        foreach (string name in accessorsByAlternativeNames.Keys) {
            accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
        }

        return accessorsByNames;
    }
}
