using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    internal class MethodInfoStore : IReadOnlyKeyValueStore<string, MethodInfo?>, ICliReadOnlyKeyValueStore<string, MethodInfo?, MethodInfo> {

        public MethodInfoStore(object obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IAccessorValidator accessorValidator) {
            ParentObject = obj;
            BindingFlags = bindingFlags;
            ClassMemberStringifier = classMemberStringifier;
            AubcommandAccessorValidator = accessorValidator;
            ParentType = obj.GetType();
            AccessorsByNames = GetAccessorsByNames();
        }

        public IEnumerable<MethodInfo> Accessors => ParentType.GetMethods(BindingFlags).Where(ContainsAccessor);
        public IAccessorValidator AubcommandAccessorValidator { get; }
        protected BindingFlags BindingFlags { get; }
        protected IClassMemberStringifier ClassMemberStringifier { get; }
        protected object ParentObject { get; }
        protected Type ParentType { get; }
        private IDictionary<string, MethodInfo> AccessorsByNames { get; }
        public MethodInfo? this[string key] {
            get {
                if (!TryGetAccessor(key, out var result)) {
                    throw new CliException($"invalid command"); // throw new MissingMemberException($"Missing {key} in {ParentType}."); ;
                }
                return result;
            }
        }

        public bool ContainsKey(string key) {
            return TryGetAccessor(key, out _);
        }

        public Type GetDataType(string key) {
            return this[key]!.ReturnType;
        }

        public IDictionary<MethodInfo, IEnumerable<string>> GetKeysByAccessors() {
            var result = AccessorsByNames
                .GroupBy(kv => kv.Value)
                .ToDictionary(
                    group => (MethodInfo)group.Key,
                    group => group.Select(kv => kv.Key)
                );
            return result;
        }

        public bool TryGetAccessor(string key, [MaybeNullWhen(false)] out MethodInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        private bool ContainsAccessor(MethodInfo accessor) {
            return accessor.ReflectedType == ParentType && AubcommandAccessorValidator.IsValid(accessor);
        }

        private IDictionary<string, MethodInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, MethodInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw new CliException($"cli name of {accessor.Name} collides with {accessorsByRequiredNames[name].Name}");
                    }

                }
            }

            var accessorsByAlternativeNames = new Dictionary<string, MethodInfo>(stringComparer);
            var collidingAlternativeNames = new HashSet<string>();
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetAlternativeNames(accessor)) {
                    if (!accessorsByAlternativeNames.TryAdd(name, accessor)) {
                        collidingAlternativeNames.Add(name);
                    }

                }
            }

            foreach (var name in collidingAlternativeNames) {
                accessorsByAlternativeNames.Remove(name);
            }

            var accessorsByNames = accessorsByRequiredNames;
            foreach (var name in accessorsByAlternativeNames.Keys) {
                accessorsByNames.TryAdd(name, accessorsByAlternativeNames[name]);
            }

            return accessorsByNames;
        }
    }
}
