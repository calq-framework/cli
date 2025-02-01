using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Extensions.System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CalqFramework.Cli.Serialization {
    public class HelpGenerator {
        protected string GetOptionKey(string key) {
            return key.Length > 1 ? $"--{key}" : $"-{key}";
        }

        public string GetHelp(IEnumerable<Option> options, IEnumerable<Command> commands, IEnumerable<Method> methods) {
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
                result += $"{method.Keys.First()}({string.Join(", ", method.PositionalParameters.Select(x => $"{ToStringHelper.GetTypeName(x.Type)} {x.Keys.First()}{(x.HasDefaultValue ? $" = {x.Value ?.ToString()!.ToLower()}" : "")}"))})";
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
                result += $" # {ToStringHelper.GetTypeName(type)} ({defaultValue?.ToString()!.ToLower()})\n";
            }

            return result;
        }
        public string GetHelp(IEnumerable<Option> options, Method method) {
            var result = "";

            result += "[DESCRIPTION]\n";
            result += ToStringHelper.GetMemberSummary(method.Methodinfo);
            result += "\n";

            result += "[POSITIONAL PARAMETERS]\n";
            foreach (var parameter in method.PositionalParameters) {
                result += $"{parameter.Keys.First()} # {ToStringHelper.GetTypeName(parameter.Type)} {parameter.Value}\n";
            }

            return result;
        }
    }
}