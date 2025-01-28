using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliClassDataMemberSerializer : ICliClassDataMemberSerializer {

        protected string MemberInfoToString(MemberInfo memberInfo, BindingFlags bindingAttr) {
            var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
            name = bindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

            var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
            shortName = bindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

            return ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}";
        }

        public string GetOptionsString(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue, BindingFlags bindingAttr) {
            var result = "";

            var options = members.Where(x => {
                return ValueParser.IsParseable(getDataType(x));
            });
            ;
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = getDataType(option);
                var defaultValue = getDataValue(option);
                result += $"{MemberInfoToString(option, bindingAttr)} # {ToStringHelper.GetTypeName(type)} ({defaultValue?.ToString()!.ToLower()})\n";
            }

            return result;
        }

        public string GetCommandsString(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue, BindingFlags bindingAttr) {
            var result = "";

            var coreCommands = members.Where(x => {
                return !ValueParser.IsParseable(getDataType(x));
            });

            result += "[CORE COMMANDS]\n";
            foreach (var command in coreCommands) {
                result += $"{MemberInfoToString(command, bindingAttr)}\n";
            }

            return result;
        }
    }
}
