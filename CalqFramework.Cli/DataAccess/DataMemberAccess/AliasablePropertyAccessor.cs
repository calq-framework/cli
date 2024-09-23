using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess
{
    // TODO unify with FieldAccessor
    internal class AliasablePropertyAccessor : PropertyAccessorBase
    {
        public AliasablePropertyAccessor(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr)
        {
        }

        public override string DataMemberToString(MemberInfo memberInfo) {
            var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
            name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

            var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
            shortName = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

            return ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}";
        }

        public override IDictionary<string, MemberInfo> GetDataMembersByKeys()
        {
            var result = new Dictionary<string, MemberInfo>();
            var membersWithShortNames = new HashSet<MemberInfo>();
            var shortNameDuplicates = new HashSet<string>();
            var members = Type.GetProperties(BindingAttr);
            foreach (var member in members)
            {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null)
                {
                    result[name] = member;
                }
                else
                {
                    result[member.Name] = member;
                }
                var shortName = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                if (shortName != null)
                {
                    result[shortName.ToString()!] = member;
                    membersWithShortNames.Add(member);
                }
            }
            foreach (var member in members)
            {
                if (!membersWithShortNames.Contains(member))
                {
                    if (result.TryAdd(member.Name[0].ToString(), member) == false)
                    {
                        shortNameDuplicates.Add(member.Name[0].ToString());
                    };
                }
            }
            foreach (var duplicate in shortNameDuplicates)
            {
                result.Remove(duplicate);
            }
            return result;
        }

        public override bool HasDataMember(MemberInfo memberInfo) {
            return memberInfo is PropertyInfo;
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        protected override MemberInfo? GetDataMemberCore(string key)
        {
            if (key.Length == 1)
            {
                foreach (var member in Type.GetProperties(BindingAttr))
                {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }
            }

            foreach (var member in Type.GetProperties(BindingAttr))
            {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key)
                {
                    return member;
                }
            }

            var dataMember = Type.GetProperty(key, BindingAttr);

            if (dataMember == null && key.Length == 1)
            {
                foreach (var member in Type.GetProperties(BindingAttr))
                {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }

                foreach (var member in Type.GetProperties(BindingAttr))
                {
                    if (member.Name[0] == key[0])
                    {
                        return member;
                    }
                }
            }

            return dataMember;
        }
    }
}