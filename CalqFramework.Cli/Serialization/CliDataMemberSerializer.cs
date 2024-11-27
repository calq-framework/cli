﻿using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliDataMemberSerializer : ICliDataMemberSerializer {
        private BindingFlags BindingAttr { get; set; }
        private Func<IEnumerable<MemberInfo>> GetMembers;
        private Func<MemberInfo, Type> GetDataType;
        private Func<MemberInfo, object?> GetDataValue;

        public CliDataMemberSerializer(BindingFlags bindingAttr, Func<IEnumerable<MemberInfo>> getMembers, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue) {
            BindingAttr = bindingAttr;
            GetMembers = getMembers;
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

        public string GetHelpString() {
            var result = "";

            var members = GetMembers().ToList();
            var options = members.Where(x => {
                return ValueParser.IsParseable(GetDataType(x));
            });
            var coreCommands = members.Where(x => {
                return !ValueParser.IsParseable(GetDataType(x));
            });

            result += "[CORE COMMANDS]\n";
            foreach (var command in coreCommands) {
                result += $"{MemberInfoToString(command)}\n";
            }

            result += "\n";
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = GetDataType(option);
                var defaultValue = GetDataValue(option);
                result += $"{MemberInfoToString(option)} # {ToStringHelper.GetTypeName(type)} ({defaultValue})\n";
            }

            //Console.WriteLine();
            //Console.WriteLine("[ACTION COMMANDS]");
            //foreach (var methodInfo in methodResolver.Methods) {
            //    Console.WriteLine(methodResolver.MethodToString(methodInfo));
            //}

            return result;
        }
    }
}
