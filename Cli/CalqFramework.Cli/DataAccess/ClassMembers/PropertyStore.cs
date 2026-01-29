using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMembers;

namespace CalqFramework.Cli.DataAccess.ClassMembers {

    internal class PropertyStore<TValue> : PropertyStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public PropertyStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessValidator accessValidator, IValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            AccessValidator = accessValidator;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        protected IClassMemberStringifier ClassMemberStringifier { get; }
        private IDictionary<string, PropertyInfo> AccessorsByNames { get; }
        private IAccessValidator AccessValidator { get; }
        private IValueConverter<TValue> ValueConverter { get; }

        public override bool ContainsAccessor(PropertyInfo accessor) {
            return accessor.ReflectedType == ParentType && AccessValidator.IsValid(accessor);
        }

        public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() {
            return AccessorsByNames
                .GroupBy(kv => kv.Value)
                .Select(g => new AccessorKeysPair<MemberInfo>(
                    (MemberInfo)g.Key,
                    g.Select(kv => kv.Key).ToArray()
                ));
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, PropertyInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor));
        }

        protected override object? ConvertToInternalValue(TValue value, PropertyInfo accessor) {
            object? currentValue;
            try {
                currentValue = GetValueOrInitialize(accessor);
            } catch (MissingMethodException) {
                currentValue = null;
            }
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor), currentValue);
        }

        private IDictionary<string, PropertyInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, PropertyInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw new CliException($"cli name of {accessor.Name} collides with {accessorsByRequiredNames[name].Name}");
                    }

                }
            }

            var accessorsByAlternativeNames = new Dictionary<string, PropertyInfo>(stringComparer);
            var collidingAlternativeNames = new HashSet<string>();
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                    if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                        collidingAlternativeNames.Add(name);
                    }

                }
            }

            foreach (var name in collidingAlternativeNames) {
                accessorsByAlternativeNames.Remove(name);
            }

            var accessorsByNames = accessorsByRequiredNames;
            foreach (var name in accessorsByAlternativeNames.Keys) {
                accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
            }

            return accessorsByNames;
        }
    }
}
