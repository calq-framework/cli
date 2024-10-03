﻿using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System.Linq;
using System.Reflection;
using CalqFramework.Serialization.DataAccess;
using System.Collections;

namespace CalqFramework.Cli.DataAccess.DataMemberAccess {
    // TODO unify with FieldAccessor
    internal class CliPropertyAccessor : PropertyAccessorBase<string> {
        public override object? this[MemberInfo dataMediator] {
            get {
                object? result = null;
                if (base[dataMediator] is not ICollection collection) {
                    result = base[dataMediator];
                } else {
                    result = new CollectionAccessor(collection)["0"]; // FIXME
                }
                return result;
            }
            set {
                if (base[dataMediator] is not ICollection collection) {
                    base[dataMediator] = value;
                } else {
                    new CollectionAccessor(collection).AddValue(value);
                }
            }
        }

        public CliPropertyAccessor(object obj, BindingFlags bindingAttr) : base(obj, bindingAttr) {
        }

        protected new string DataMediatorToString(MemberInfo memberInfo) {
            var name = memberInfo.GetCustomAttribute<NameAttribute>()?.Name ?? memberInfo.Name;
            name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? name.ToLower() : name;

            var shortName = memberInfo.GetCustomAttribute<ShortNameAttribute>()?.Name ?? memberInfo.Name[0];
            shortName = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? shortName.ToString().ToLower()[0] : shortName;

            return ValueParser.IsParseable(memberInfo.GetUnderlyingType()) ? $"--{name}, -{shortName}" : $"{name}";
        }

        // FIXME do not assign the first occurances - check for duplicates. if duplicate found then then return null
        protected override MemberInfo? GetClassMember(string key) {
            if (key.Length == 1) {
                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<ShortNameAttribute>()?.Name;
                    if (name != null && name == key[0]) {
                        return member;
                    }
                }
            }

            foreach (var member in ParentType.GetProperties(BindingAttr)) {
                var name = member.GetCustomAttribute<NameAttribute>()?.Name;
                if (name != null && name == key) {
                    return member;
                }
            }

            var dataMember = ParentType.GetProperty(key, BindingAttr);

            if (dataMember == null && key.Length == 1) {
                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    var name = member.GetCustomAttribute<NameAttribute>()?.Name[0];
                    if (name != null && name == key[0]) {
                        return member;
                    }
                }

                foreach (var member in ParentType.GetProperties(BindingAttr)) {
                    if (member.Name[0] == key[0]) {
                        return member;
                    }
                }
            }

            return dataMember;
        }

        public override string ToString() {
            var result = "";

            var members = DataMediators.ToList();
            var options = members.Where(x => {
                return ValueParser.IsParseable(GetDataType(x));
            });
            var coreCommands = members.Where(x => {
                return !ValueParser.IsParseable(GetDataType(x));
            });

            result += "[CORE COMMANDS]\n";
            foreach (var command in coreCommands) {
                result += $"{DataMediatorToString(command)}\n";
            }

            result += "\n";
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = GetDataType(option);
                var defaultValue = this[option];
                result += $"{DataMediatorToString(option)} # {ToStringHelper.GetTypeName(type)} ({defaultValue})\n";
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