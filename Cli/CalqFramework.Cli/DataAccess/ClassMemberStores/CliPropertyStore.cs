using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMemberStores;
using CalqFramework.DataAccess.CollectionElementStores;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores {

    internal class CliPropertyStore<TValue> : PropertyStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public CliPropertyStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessValidator accessValidator, ICompositeValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            AccessValidator = accessValidator;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        protected IClassMemberStringifier ClassMemberStringifier { get; }
        private IDictionary<string, PropertyInfo> AccessorsByNames { get; }
        private IAccessValidator AccessValidator { get; }
        private ICompositeValueConverter<TValue> ValueConverter { get; }

        public override bool ContainsAccessor(PropertyInfo accessor) {
            return accessor.ReflectedType == ParentType && AccessValidator.IsValid(accessor);
        }

        public override Type GetDataType(PropertyInfo accessor) {
            return ValueConverter.GetDataType(accessor.PropertyType);
        }

        public IEnumerable<AccessorKeysPair<MemberInfo>> GetAccessorKeysPairs() {
            return AccessorsByNames
                .GroupBy(kv => kv.Value)
                .Select(g => new AccessorKeysPair<MemberInfo>(
                    (MemberInfo)g.Key,
                    g.Select(kv => kv.Key).ToArray()
                ));
        }

        public bool IsCollection(string key) {
            PropertyInfo accessor = GetAccessor(key);
            return ValueConverter.IsCollection(accessor.PropertyType);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, PropertyInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, accessor.PropertyType);
        }

        protected override object? ConvertToInternalValue(TValue value, PropertyInfo accessor) {
            object? currentValue;
            try {
                currentValue = GetValueOrInitialize(accessor);
            } catch (MissingMethodException) {
                currentValue = null;
            }
            
            return ValueConverter.ConvertToInternalValue(value, accessor.PropertyType, currentValue);
        }

        private IDictionary<string, PropertyInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, PropertyInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw CliErrors.NameCollision(accessor.Name, accessorsByRequiredNames[name].Name);
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
