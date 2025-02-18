using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    internal class FieldStore<TValue> : FieldStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public FieldStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessorValidator cliValidator, IValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            CliValidator = cliValidator;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        private IDictionary<string, FieldInfo> AccessorsByNames { get; }
        private IClassMemberStringifier ClassMemberStringifier { get; }
        private IAccessorValidator CliValidator { get; }
        private IValueConverter<TValue> ValueConverter { get; }
        public override object? this[FieldInfo accessor] {
            get {
                object? result;
                if (base[accessor] is not ICollection collection) {
                    result = base[accessor];
                } else {
                    result = collection;
                }
                return result;
            }
            set {
                if (base[accessor] is not ICollection collection) {
                    base[accessor] = value;
                } else {
                    new CollectionStore(collection).AddValue(value);
                }
            }
        }

        public override bool ContainsAccessor(FieldInfo accessor) {
            return accessor is not null && accessor.DeclaringType == ParentType && CliValidator.IsValid(accessor);
        }

        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            var result = AccessorsByNames
                .GroupBy(kv => kv.Value)
                .ToDictionary(
                    group => (MemberInfo)group.Key,
                    group => group.Select(kv => kv.Key)
                );
            return result;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, FieldInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor));
        }

        protected override object? ConvertToInternalValue(TValue value, FieldInfo accessor) {
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor));
        }

        private IDictionary<string, FieldInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, FieldInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw new CliException($"cli name of {accessor.Name} collides with {accessorsByRequiredNames[name].Name}");
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
