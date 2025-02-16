using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    internal class MethodInfoStore : IReadOnlyKeyValueStore<string, MethodInfo?>, ICliReadOnlyKeyValueStore<string, MethodInfo?, MethodInfo> {

        public MethodInfoStore(object obj, BindingFlags bindingAttr) {
            ParentObject = obj;
            BindingAttr = bindingAttr;
            ParentType = obj.GetType();
        }

        public IEnumerable<MethodInfo> Accessors => ParentType.GetMethods(BindingAttr).Where(ContainsAccessor);
        protected BindingFlags BindingAttr { get; }
        protected object ParentObject { get; }
        protected Type ParentType { get; }

        public MethodInfo? this[string key] {
            get {
                var result = GetClassMember(key) ?? throw new CliException($"invalid command"); // throw new MissingMemberException($"Missing {key} in {ParentType}."); ;
                return result;
            }
        }

        public bool ContainsKey(string key) {
            var result = GetClassMember(key);
            return result != null;
        }

        public Type GetDataType(string key) {
            return this[key]!.ReturnType;
        }

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MethodInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MethodInfo, IEnumerable<string>>();
            foreach (var accessor in Accessors) {
                var accesorKeys = new List<string>();
                foreach (var atribute in accessor.GetCustomAttributes<CliNameAttribute>()) {
                    accesorKeys.Add(atribute.Name);
                }
                if (accesorKeys.Count == 0) {
                    accesorKeys.Add(accessor.Name);
                }
                if (accesorKeys.Select(x => x.Length == 1).Count() == 0) {
                    accesorKeys.Add(accesorKeys[0][0].ToString());
                }
                keys[accessor] = accesorKeys;
            }
            return keys;
        }

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }

        private bool ContainsAccessor(MethodInfo accessor) {
            return accessor is MethodInfo && accessor.DeclaringType == ParentType && !IsDotnetSpecific(accessor);
        }

        // FIXME align with GetKeysByAccessors
        private MethodInfo? GetClassMember(string key) {
            var stringComparison = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var member in Accessors) {
                foreach (var atribute in member.GetCustomAttributes<CliNameAttribute>()) {
                    if (string.Equals(atribute.Name, key, stringComparison)) {
                        return member;
                    }
                }
                foreach (var atribute in member.GetCustomAttributes<CliNameAttribute>()) {
                    if (string.Equals(atribute.Name[0].ToString(), key, stringComparison)) {
                        return member;
                    }
                }
                if (string.Equals(member.Name, key, stringComparison)) {
                    return member;
                }
                if (string.Equals(member.Name[0].ToString(), key, stringComparison)) {
                    return member;
                }
            }

            return null;
        }
    }
}