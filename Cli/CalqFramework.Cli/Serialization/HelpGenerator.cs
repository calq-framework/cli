using CalqFramework.Cli.InterfaceComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CalqFramework.Cli.Serialization {
    public class HelpGenerator {
        protected string GetOptionKey(string key) {
            return key.Length > 1 ? $"--{key}" : $"-{key}";
        }

        public string GetHelp(IEnumerable<Option> options, IEnumerable<Submodule> commands, IEnumerable<Subcommand> methods) {
            var result = "";

            result += "[CORE COMMANDS]\n";
            foreach (var command in commands) {
                result += $"{command.Keys.First()}";
                foreach (var key in command.Keys.Skip(1)) {
                    result += $", {key}";
                }
                result += "\n";
            }
            result += "\n";


            result += "[ACTION COMMANDS]";
            foreach (var method in methods) {
                result += $"{method.Keys.First()}({string.Join(", ", method.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Keys.First()}{(x.HasDefaultValue ? $" = {x.Value ?.ToString()!.ToLower()}" : "")}"))})";
                result += "\n";
            }
            result += "\n";

            ;
            result += "[OPTIONS]\n";
            foreach (var option in options) {
                var type = option.Type;
                var defaultValue = option.Value;
                result += $"{GetOptionKey(option.Keys.First())}";
                foreach (var key in option.Keys.Skip(1)) {
                    result += $", {GetOptionKey(key)}";
                }
                result += $" # {GetTypeName(type)} ({defaultValue?.ToString()!.ToLower()})\n";
            }

            return result;
        }
        public string GetHelp(IEnumerable<Option> options, Subcommand method) {
            var result = "";

            result += "[DESCRIPTION]\n";
            result += GetMemberSummary(method.MethodInfo);
            result += "\n";

            result += "[POSITIONAL PARAMETERS]\n";
            foreach (var parameter in method.Parameters) {
                result += $"{parameter.Keys.First()} # {GetTypeName(parameter.Type)} {parameter.Value}\n";
            }

            return result;
        }

        private string GetTypeName(Type type) {
            if (type.IsGenericType) {
                string genericArguments = string.Join(", ", type.GetGenericArguments().Select(x => x.Name.ToLower()).ToList());
                return $"{type.Name.ToLower().Substring(0, type.Name.ToLower().IndexOf("`"))}<{genericArguments}>";
            }
            return type.Name.ToLower();
        }

        // TODO option to turn on and off
        // FIXME handle FileNotFoundException
        private string GetMemberSummary(MemberInfo memberInfo) {
            var xmlFilePath = Path.ChangeExtension(memberInfo.Module.Assembly.Location, "xml");

            string memberName;
            switch (memberInfo) {
                case MethodInfo method:
                    memberName = $"M:{method.DeclaringType?.FullName}.{method.Name}";
                    var parameters = method.GetParameters();
                    if (parameters.Length > 0) {
                        memberName += $"({string.Join(",", parameters.Select(p => p.ParameterType.FullName))})";
                    }
                    break;
                case PropertyInfo property:
                    memberName = $"P:{property.DeclaringType?.FullName}.{property.Name}";
                    break;
                case FieldInfo field:
                    memberName = $"F:{field.DeclaringType?.FullName}.{field.Name}";
                    break;
                default:
                    throw new Exception("Unsupported member type.");
            }

            var doc = XDocument.Load(xmlFilePath);
            var summary = doc.Descendants("member")
                .FirstOrDefault(m => m.Attribute("name")?.Value == memberName)?
                .Element("summary")?.Value;

            return $"{summary?.Trim()}\n" ?? "\n";
        }
    }
}