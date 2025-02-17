﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    internal class PropertyStore<TValue> : PropertyStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public PropertyStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessorValidator cliValidator, IValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            CliValidator = cliValidator;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        protected IClassMemberStringifier ClassMemberStringifier { get; }
        private IDictionary<string, PropertyInfo> AccessorsByNames { get; }
        private IAccessorValidator CliValidator { get; }
        private IValueConverter<TValue> ValueConverter { get; }

        public override object? this[PropertyInfo accessor] {
            get {
                object? result;
                if (base[accessor] is not ICollection collection) {
                    result = base[accessor];
                } else {
                    result = new CollectionStore(collection)["0"]; // FIXME return whole collection instead just first element. this is used for reading default value so print whole collection
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

        public override bool ContainsAccessor(PropertyInfo accessor) {
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

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, PropertyInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor)); // value?.ToString()?.ToLower();
        }

        protected override object? ConvertToInternalValue(TValue value, PropertyInfo accessor) {
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor)); // ValueParser.ParseValue(value, GetDataType(accessor));
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
