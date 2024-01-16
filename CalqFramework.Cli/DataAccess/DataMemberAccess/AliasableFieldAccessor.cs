using CalqFramework.Cli.Attributes;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess
{
    // TODO unify with PropertyAccessor
    internal class AliasableFieldAccessor : FieldAccessorBase
    {
        public AliasableFieldAccessor(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr)
        {
        }

        public override IDictionary<string, MemberInfo> GetDataMembersByKeys()
        {
            var result = new Dictionary<string, MemberInfo>();
            var membersWithShortNames = new HashSet<MemberInfo>();
            var shortNameDuplicates = new HashSet<string>();
            var members = Type.GetFields(BindingAttr);
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

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        protected override MemberInfo? GetDataMemberCore(string key)
        {
            if (key.Length == 1)
            {
                foreach (var member in Type.GetFields(BindingAttr))
                {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }
            }

            foreach (var member in Type.GetFields(BindingAttr))
            {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key)
                {
                    return member;
                }
            }

            var dataMember = Type.GetField(key, BindingAttr);

            if (dataMember == null && key.Length == 1)
            {
                foreach (var member in Type.GetFields(BindingAttr))
                {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == key[0])
                    {
                        return member;
                    }
                }

                foreach (var member in Type.GetFields(BindingAttr))
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