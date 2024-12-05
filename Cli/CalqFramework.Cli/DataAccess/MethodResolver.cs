using CalqFramework.Cli.Attributes;
using CalqFramework.Cli.Serialization;
using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.Serialization.DataAccess.ClassMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CalqFramework.Cli.DataAccess
{
    internal class MethodResolver
    {
        public object Obj { get; }
        public BindingFlags BindingAttr { get; }
        public IEnumerable<MethodInfo> Methods { get => Obj.GetType().GetMethods(BindingAttr).Where(x => !IsDotnetSpecific(x)); }

        public MethodResolver(object obj, BindingFlags methodBindingAttr)
        {
            Obj = obj;
            BindingAttr = methodBindingAttr;
        }

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }

        public MethodInfo GetMethod(string key) {
            var result = Obj.GetType().GetMethod(key, BindingAttr);
            if (result != null && !IsDotnetSpecific(result)) {
                return result;
            } else {
                throw new CliException($"invalid command");
            }
        }

        public string MethodToString(MethodInfo method) {
            var name = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? method.Name!.ToLower() : method.Name;
            return $"{name}({string.Join(", ", method.GetParameters().Select(x => $"{ToStringHelper.GetTypeName(x.ParameterType)} {x.Name}{(x.HasDefaultValue ? $" = {x.DefaultValue?.ToString()!.ToLower()}" : "")}"))})";
        }
    }
}