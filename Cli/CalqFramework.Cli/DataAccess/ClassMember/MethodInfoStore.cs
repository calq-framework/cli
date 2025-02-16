using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    internal class MethodInfoStore : IReadOnlyKeyValueStore<string, MethodInfo?>, ICliReadOnlyKeyValueStore<string, MethodInfo?, MethodInfo> {

        public MethodInfoStore(object obj, BindingFlags bindingAttr, IClassMemberStringifier classMemberStringifier) {
            ParentObject = obj;
            BindingAttr = bindingAttr;
            ClassMemberStringifier = classMemberStringifier;
            ParentType = obj.GetType();
        }

        public IEnumerable<MethodInfo> Accessors => ParentType.GetMethods(BindingAttr).Where(ContainsAccessor);
        protected BindingFlags BindingAttr { get; }
        protected IClassMemberStringifier ClassMemberStringifier { get; }
        protected object ParentObject { get; }
        protected Type ParentType { get; }

        public MethodInfo? this[string key] {
            get {
                MethodInfo result = GetClassMember(key) ?? throw new CliException($"invalid command"); // throw new MissingMemberException($"Missing {key} in {ParentType}."); ;
                return result;
            }
        }

        public bool ContainsKey(string key) {
            MethodInfo? result = GetClassMember(key);
            return result != null;
        }

        public Type GetDataType(string key) {
            return this[key]!.ReturnType;
        }

        // TODO two-pass, first pass find collisions and exclude them from second pass when building the dictionary
        public IDictionary<MethodInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<MethodInfo, IEnumerable<string>>();
            foreach (MethodInfo accessor in Accessors) {
                keys[accessor] = ClassMemberStringifier.GetNames(accessor);
            }
            return keys;
        }

        private static bool IsDotnetSpecific(MethodInfo methodInfo) {
            return methodInfo.DeclaringType == typeof(object) || methodInfo.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
        }

        private bool ContainsAccessor(MethodInfo accessor) {
            return accessor is not null && accessor.DeclaringType == ParentType && !IsDotnetSpecific(accessor);
        }

        // FIXME align with GetKeysByAccessors
        private MethodInfo? GetClassMember(string key) {
            StringComparison stringComparison = BindingAttr.HasFlag(BindingFlags.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (MethodInfo member in Accessors) {
                if (ClassMemberStringifier.GetNames(member).Where(x => string.Equals(x, key, stringComparison)).Any()) {
                    return member;
                }
            }

            return null;
        }
    }
}
