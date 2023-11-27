using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using System;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class PropertyAccessor : PropertyAccessorBase {
        public PropertyAccessor(BindingFlags bindingAttr) : base(bindingAttr) {
        }

        public override MemberInfo? GetDataMember(Type type, string dataMemberKey) {
            foreach (var member in type.GetProperties()) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == dataMemberKey) {
                    return member;
                }
            }

            return type.GetProperty(dataMemberKey, BindingAttr);
        }
    }
}