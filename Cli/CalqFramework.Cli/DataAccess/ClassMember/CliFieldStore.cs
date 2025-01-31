using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    // TODO unify with PropertyStore
    internal class CliFieldStore : FieldStoreBase<string>, ICliOptionsStore
    {
        private ICliClassDataMemberSerializer CliSerializer { get; }

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

        public CliFieldStore(object obj, BindingFlags bindingAttr, ICliClassDataMemberSerializer cliSerializer) : base(obj, bindingAttr)
        {
            CliSerializer = cliSerializer;
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        private MemberInfo? GetClassMember(string key) {
            if (key.Length == 1)
            {
                foreach (var member in ParentType.GetFields(BindingAttr))
                {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }
            }

            foreach (var member in ParentType.GetFields(BindingAttr))
            {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key)
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
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }

                foreach (var member in ParentType.GetFields(BindingAttr))
                {
                    if (member.Name[0] == key[0])
                    {
                        return member;
                    }
                }
            }

            return dataMember;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MemberInfo result) {
            result = GetClassMember(key);
            return result != null;
        }

        public override bool ContainsAccessor(MemberInfo accessor) {
            return accessor is FieldInfo && accessor.DeclaringType == ParentType;
        }

        public string GetCommandsString() {
            return CliSerializer.GetCommandsString(Accessors, (x) => GetDataType(x), (x) => this[x], BindingAttr);
        }

        public string GetOptionsString() {
            return CliSerializer.GetOptionsString(Accessors, (x) => GetDataType(x), (x) => this[x], BindingAttr);
        }
    }
}