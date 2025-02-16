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

    // TODO unify with PropertyStore
    internal class FieldStore<TValue> : FieldStoreBase<string, TValue>, ICliKeyValueStore<string, TValue, MemberInfo> {

        public FieldStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessorValidator cliValidator, IValueConverter<TValue> valueConverter) : base(obj, bindingFlags) {
            ClassMemberStringifier = classMemberStringifier;
            CliValidator = cliValidator;
            ValueConverter = valueConverter;
        }

        private IClassMemberStringifier ClassMemberStringifier { get; }
        private IAccessorValidator CliValidator { get; }
        private IValueConverter<TValue> ValueConverter { get; }

        public override object? this[FieldInfo accessor] {
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

        public override bool ContainsAccessor(FieldInfo accessor) {
            return accessor is not null && accessor.DeclaringType == ParentType && CliValidator.IsValid(accessor);
        }

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MemberInfo, IEnumerable<string>>();
            foreach (FieldInfo accessor in Accessors) {
                keys[accessor] = ClassMemberStringifier.GetNames(accessor);
            }
            return keys;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out FieldInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        protected override TValue ConvertFromInternalValue(object? value, FieldInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor));
        }

        protected override object? ConvertToInternalValue(TValue value, FieldInfo accessor) {
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor));
        }

        private FieldInfo? GetClassMember(string key) {
            StringComparison stringComparison = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (FieldInfo member in Accessors) {
                if (ClassMemberStringifier.GetNames(member).Where(x => string.Equals(x, key, stringComparison)).Any()) {
                    return member;
                }
            }

            return null;
        }
    }
}
