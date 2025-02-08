using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    // TODO unify with PropertyStore
    internal class CliFieldStore : FieldStoreBase<string>, ICliStore<string, object?, MemberInfo> {
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

        public CliFieldStore(object obj, BindingFlags bindingAttr, IClassMemberSerializer cliSerializer, IAccessorValidator cliValidator) : base(obj, bindingAttr)
        {
            CliSerializer = cliSerializer;
            CliValidator = cliValidator;
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        private MemberInfo? GetClassMember(string key) {
            if (key.Length == 1)
            {
                foreach (var member in ParentType.GetFields(BindingAttr))
                {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0] && ContainsAccessor(member))
                    {
                        return member;
                    }
                }
            }

            foreach (var member in ParentType.GetFields(BindingAttr))
            {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key && ContainsAccessor(member))
                {
                    return member;
                }
            }

            var dataMember = ParentType.GetField(key, BindingAttr);

            if (dataMember == null && key.Length == 1)
            {
                foreach (var member in ParentType.GetFields(BindingAttr))
                {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == key[0] && ContainsAccessor(member))
                    {
                        return member;
                    }
                }

                foreach (var member in ParentType.GetFields(BindingAttr))
                {
                    if (member.Name[0] == key[0] && ContainsAccessor(member))
                    {
                        return member;
                    }
                }
            }

            return dataMember != null == ContainsAccessor(dataMember!) ? dataMember : null;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is FieldInfo && accessor.DeclaringType == ParentType && CliValidator.IsValid(accessor);
        }

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MemberInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MemberInfo, IEnumerable<string>>();
            foreach (var accessor in Accessors) {
                var accesorKeys = new List<string>();
                foreach (var atribute in accessor.GetCustomAttributes<NameAttribute>()) {
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