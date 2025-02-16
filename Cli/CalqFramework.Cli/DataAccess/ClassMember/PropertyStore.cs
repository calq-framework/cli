using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    // TODO unify with FieldStore
    internal class PropertyStore<TValue> : PropertyStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public PropertyStore(object obj, BindingFlags bindingAttr, IClassMemberSerializer cliSerializer, IAccessorValidator cliValidator, IValueConverter<TValue> valueConverter) : base(obj, bindingAttr) {
            CliSerializer = cliSerializer;
            CliValidator = cliValidator;
            ValueConverter = valueConverter;
        }

        private IClassMemberSerializer CliSerializer { get; }
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

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MemberInfo, IEnumerable<string>>();
            foreach (PropertyInfo accessor in Accessors) {
                var accesorKeys = new List<string>();
                foreach (CliNameAttribute atribute in accessor.GetCustomAttributes<CliNameAttribute>()) {
                    accesorKeys.Add(atribute.Name);
                }
                if (accesorKeys.Count == 0) {
                    accesorKeys.Add(accessor.Name);
                }
                if (!accesorKeys.Select(x => x.Length == 1).Any()) {
                    accesorKeys.Add(accesorKeys[0][0].ToString());
                }
                keys[accessor] = accesorKeys;
            }
            return keys;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out PropertyInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        protected override TValue ConvertFromInternalValue(object? value, PropertyInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor)); // value?.ToString()?.ToLower();
        }

        protected override object? ConvertToInternalValue(TValue value, PropertyInfo accessor) {
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor)); // ValueParser.ParseValue(value, GetDataType(accessor));
        }

        // FIXME align with GetKeysByAccessors
        private PropertyInfo? GetClassMember(string key) {
            foreach (PropertyInfo member in Accessors) {
                foreach (CliNameAttribute atribute in member.GetCustomAttributes<CliNameAttribute>()) {
                    if (atribute.Name == key) {
                        return member;
                    }
                }
                foreach (CliNameAttribute atribute in member.GetCustomAttributes<CliNameAttribute>()) {
                    if (atribute.Name[0].ToString() == key) {
                        return member;
                    }
                }
                if (member.Name == key) {
                    return member;
                }
                if (member.Name[0].ToString() == key) {
                    return member;
                }
            }

            return null;
        }
    }
}
