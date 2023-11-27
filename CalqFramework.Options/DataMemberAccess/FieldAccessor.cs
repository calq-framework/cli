using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using System;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class FieldAccessor : FieldAccessorBase {
        public FieldAccessor(BindingFlags bindingAttr) : base(bindingAttr) {
        }

        public override MemberInfo? GetDataMember(Type type, string dataMemberKey) {
            foreach (var member in type.GetFields()) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == dataMemberKey) {
                    return member;
                }
            }

            return type.GetField(dataMemberKey, BindingAttr);
        }
    }
}