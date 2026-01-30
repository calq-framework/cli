using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMembers;

namespace CalqFramework.Cli.DataAccess.ClassMembers {

    internal class MethodExecutor<TValue> : MethodExecutorBase<string, TValue>, ICliFunctionExecutor<string, TValue, ParameterInfo> {

        public MethodExecutor(MethodInfo method, object? obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, IValueConverter<TValue> valueConverter) : base(method, obj) {
            BindingFlags = bindingFlags;
            ClassMemberStringifier = classMemberStringifier;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        protected BindingFlags BindingFlags { get; }
        protected IClassMemberStringifier ClassMemberStringifier { get; }
        private IDictionary<string, ParameterInfo> AccessorsByNames { get; }
        private IValueConverter<TValue> ValueConverter { get; }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public IEnumerable<AccessorKeysPair<ParameterInfo>> GetAccessorKeysPairs() {
            return AccessorsByNames
                .GroupBy(kv => kv.Value)
                .OrderBy(g => ((ParameterInfo)g.Key).Position)
                .Select(g => new AccessorKeysPair<ParameterInfo>(
                    (ParameterInfo)g.Key,
                    g.Select(kv => kv.Key).ToArray()
                ));
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, ParameterInfo accessor) {
            return ValueConverter.ConvertFromInternalValue(value, GetDataType(accessor));
        }

        protected override object? ConvertToInternalValue(TValue value, ParameterInfo accessor) {
            object? currentValue;
            try {
                currentValue = GetValueOrInitialize(accessor);
            } catch (MissingMethodException) {
                currentValue = null;
            }
            return ValueConverter.ConvertToInternalValue(value, GetDataType(accessor), currentValue);
        }

        private IDictionary<string, ParameterInfo> GetAccessorsByNames() {
            var stringComparer = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

            var accessorsByRequiredNames = new Dictionary<string, ParameterInfo>(stringComparer);
            foreach (var accessor in Accessors) {
                foreach (var name in ClassMemberStringifier.GetRequiredNames(accessor)) {
                    if (!accessorsByRequiredNames.TryAdd(name, accessor)) {
                        throw CliErrors.NameCollision(accessor.Name!, accessorsByRequiredNames[name].Name!);
                    }

                }
            }

            var accessorsByAlternativeNames = new Dictionary<string, ParameterInfo>(stringComparer);
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
