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

    internal class CliFieldStore<TValue> : FieldStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public CliFieldStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessValidator accessValidator, ICompositeValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            AccessValidator = accessValidator;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        private IDictionary<string, FieldInfo> AccessorsByNames { get; }
        private IAccessValidator AccessValidator { get; }
        private IClassMemberStringifier ClassMemberStringifier { get; }
        private ICompositeValueConverter<TValue> ValueConverter { get; }

        public override bool ContainsAccessor(FieldInfo accessor) {
            return accessor.ReflectedType == ParentType && AccessValidator.IsValid(accessor);
        }

        public override Type GetDataType(FieldInfo accessor) {
            return ValueConverter.GetDataType(accessor.FieldType);
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
            FieldInfo accessor = GetAccessor(key);
            return ValueConverter.IsCollection(accessor.FieldType);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, FieldInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, accessor.FieldType);
        }

        protected override object? ConvertToInternalValue(TValue value, FieldInfo accessor) {
            object? currentValue;
            try {
                currentValue = GetValueOrInitialize(accessor);
            } catch (MissingMethodException) {
                currentValue = null;
            }
            
            return ValueConverter.ConvertToInternalValue(value, accessor.FieldType, currentValue);
        }

        private IDictionary<string, FieldInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, FieldInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw CliErrors.NameCollision(accessor.Name, accessorsByRequiredNames[name].Name);
                    }

                }
            }

            var accessorsByAlternativeNames = new Dictionary<string, FieldInfo>(stringComparer);
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
