using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal static class DataMemberAccessHelper
    {
        public static void ToHelpString(IEnumerable<MemberInfo> memberInfos, BindingFlags BindingAttr) {
            foreach (var memberInfo in memberInfos) {
                var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
                name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

                var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
                shortName = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

                Console.WriteLine(ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}");
            }
        }
    }
}