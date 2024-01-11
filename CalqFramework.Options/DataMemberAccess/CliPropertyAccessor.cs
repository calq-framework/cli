﻿using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    // TODO unify with CliFieldAccessor
    public class CliPropertyAccessor : PropertyAccessorBase {
        public CliPropertyAccessor(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        public override IDictionary<string, MemberInfo> GetDataMembersByKeys() {
            var result = new Dictionary<string, MemberInfo>();
            var membersWithShortNames = new HashSet<MemberInfo>();
            var shortNameDuplicates = new HashSet<string>();
            var members = Type.GetProperties(BindingAttr);
            foreach (var member in members) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null) {
                    result[name] = member;
                } else {
                    result[member.Name] = member;
                }
                var shortName = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                if (shortName != null) {
                    result[shortName.ToString()!] = member;
                    membersWithShortNames.Add(member);
                }
            }
            foreach (var member in members) {
                if (!membersWithShortNames.Contains(member)) {
                    if (result.TryAdd(member.Name[0].ToString(), member) == false) {
                        shortNameDuplicates.Add(member.Name[0].ToString());
                    };
                }
            }
            foreach (var duplicate in shortNameDuplicates) {
                result.Remove(duplicate);
            }
            return result;
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        public override MemberInfo? GetDataMember(string dataMemberKey) {
            if (dataMemberKey.Length == 1) {
                foreach (var member in Type.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == dataMemberKey[0]) {
                        return member;
                    }
                }
            }

            foreach (var member in Type.GetProperties(BindingAttr)) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == dataMemberKey) {
                    return member;
                }
            }

            var dataMember = Type.GetProperty(dataMemberKey, BindingAttr);

            if (dataMember == null && dataMemberKey.Length == 1) {
                foreach (var member in Type.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == dataMemberKey[0]) {
                        return member;
                    }
                }

                foreach (var member in Type.GetProperties(BindingAttr)) {
                    if (member.Name[0] == dataMemberKey[0]) {
                        return member;
                    }
                }
            }

            return dataMember;
        }
    }
}