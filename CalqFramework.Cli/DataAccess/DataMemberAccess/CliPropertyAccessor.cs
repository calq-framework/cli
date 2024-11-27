using CalqFramework.Cli.Attributes;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System.Reflection;
using CalqFramework.Serialization.DataAccess;
using System.Collections;
using CalqFramework.Cli.Serialization;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    // TODO unify with FieldAccessor
    internal class CliPropertyAccessor : PropertyAccessorBase<string>, ICliDataMemberAccessor {
        public ICliDataMemberSerializer CliSerializer { get; }

        public override object? this[MemberInfo dataMediator] {
            get {
                object? result = null;
                if (base[dataMediator] is not ICollection collection) {
                    result = base[dataMediator];
                } else {
                    result = new CollectionAccessor(collection)["0"]; // FIXME return whole collection instead just first element. this is used for reading default value so print whole collection
                }
                return result;
            }
            set {
                if (base[dataMediator] is not ICollection collection) {
                    base[dataMediator] = value;
                } else {
                    new CollectionAccessor(collection).AddValue(value);
                }
            }
        }

        public CliPropertyAccessor(object obj, BindingFlags bindingAttr, ICliDataMemberSerializerFactory cliSerializerFactory) : base(obj, bindingAttr) {
            CliSerializer = cliSerializerFactory.CreateCliSerializer(() => DataMediators, (x) => GetDataType(x), (x) => this[x]);
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        protected override MemberInfo? GetClassMember(string key) {
            if (key.Length == 1) {
                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0]) {
                        return member;
                    }
                }
            }

            foreach (var member in ParentType.GetProperties(BindingAttr)) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key) {
                    return member;
                }
            }

            var dataMember = ParentType.GetProperty(key, BindingAttr);

            if (dataMember == null && key.Length == 1) {
                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == key[0]) {
                        return member;
                    }
                }

                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    if (member.Name[0] == key[0]) {
                        return member;
                    }
                }
            }

            return dataMember;
        }
    }
}