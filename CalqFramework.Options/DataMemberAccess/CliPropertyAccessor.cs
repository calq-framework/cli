using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using System;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class CliPropertyAccessor : PropertyAccessorBase {
        public CliPropertyAccessor(BindingFlags bindingAttr) : base(bindingAttr) {
        }

        public override MemberInfo? GetDataMember(Type type, string dataMemberKey) {
            if (dataMemberKey.Length == 1) {
                foreach (var member in type.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == dataMemberKey[0]) {
                        return member;
                    }
                }
            }

            foreach (var member in type.GetProperties(BindingAttr)) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == dataMemberKey) {
                    return member;
                }
            }

            var dataMember = type.GetProperty(dataMemberKey, BindingAttr);

            if (dataMember == null && dataMemberKey.Length == 1) {
                foreach (var member in type.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == dataMemberKey[0]) {
                        return member;
                    }
                }

                foreach (var member in type.GetProperties(BindingAttr)) {
                    if (member.Name[0] == dataMemberKey[0]) {
                        return member;
                    }
                }
            }

            return dataMember;
        }
    }
}