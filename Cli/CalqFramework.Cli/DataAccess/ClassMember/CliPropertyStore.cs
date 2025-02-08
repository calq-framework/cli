using System.Reflection;
using System.Collections;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    // TODO unify with FieldStore
    internal class CliPropertyStore : PropertyStoreBase<string>, ICliStore<string, object?, MemberInfo> {
        private IClassMemberSerializer CliSerializer { get; }
        private IAccessorValidator CliValidator { get; }

        public override object? this[MemberInfo accessor] {
            get {
                object? result = null;
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

        public CliPropertyStore(object obj, BindingFlags bindingAttr, IClassMemberSerializer cliSerializer, IAccessorValidator cliValidator) : base(obj, bindingAttr) {
            CliSerializer = cliSerializer;
            CliValidator = cliValidator;
        }

        // FIXME align with GetKeysByAccessors
        private MemberInfo? GetClassMember(string key) {
            foreach (var member in Accessors) {
                foreach (var atribute in member.GetCustomAttributes<CliNameAttribute>()) {
                    if (atribute.Name == key) {
                        return member;
                    }
                }
                foreach (var atribute in member.GetCustomAttributes<CliNameAttribute>()) {
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

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is PropertyInfo && accessor.DeclaringType == ParentType && CliValidator.IsValid(accessor);
        }

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MemberInfo, IEnumerable<string>>();
            foreach (var accessor in Accessors) {
                var accesorKeys = new List<string>();
                foreach (var atribute in accessor.GetCustomAttributes<CliNameAttribute>()) {
                    accesorKeys.Add(atribute.Name);
                }
                if (accesorKeys.Count == 0) {
                    accesorKeys.Add(accessor.Name);
                }
                if (accesorKeys.Select(x => x.Length == 1).Count() == 0) {
                    accesorKeys.Add(accesorKeys[0][0].ToString());
                }
                keys[accessor] = accesorKeys;
            }
            return keys;
        }
    }
}