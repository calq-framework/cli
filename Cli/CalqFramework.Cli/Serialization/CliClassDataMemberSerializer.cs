using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliClassDataMemberSerializer : ICliClassDataMemberSerializer {
        private BindingFlags BindingAttr { get; set; }
        private IEnumerable<MemberInfo> Members;
        private Func<MemberInfo, Type> GetDataType;
        private Func<MemberInfo, object?> GetDataValue;

        public CliClassDataMemberSerializer(BindingFlags bindingAttr, IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue) {
            BindingAttr = bindingAttr;
            Members = members;
            GetDataType = getDataType;
            GetDataValue = getDataValue;
        }

        protected string MemberInfoToString(MemberInfo memberInfo) {
            var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
            name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

            var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
            shortName = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

            return ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}";
        }

        public string GetOptionsString() {
            var result = "";

            var members = Members.ToList();
            var options = members.Where(x => {
                return ValueParser.IsParseable(GetDataType(x));
            });
;
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = GetDataType(option);
                var defaultValue = GetDataValue(option);
                result += $"{MemberInfoToString(option)} # {ToStringHelper.GetTypeName(type)} ({defaultValue?.ToString()!.ToLower()})\n";
            }

            return result;
        }

        public string GetCommandsString() {
            var result = "";

            var members = Members.ToList();
            var coreCommands = members.Where(x => {
                return !ValueParser.IsParseable(GetDataType(x));
            });

            result += "[CORE COMMANDS]\n";
            foreach (var command in coreCommands) {
                result += $"{MemberInfoToString(command)}\n";
            }

            return result;
        }
    }
}
