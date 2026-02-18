using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.Formatting;
using CalqFramework.DataAccess.ClassMemberStores;
using CalqFramework.DataAccess.CollectionElementStores;
using CalqFramework.Extensions.System;

namespace CalqFramework.Cli.DataAccess.ClassMemberStores {

    internal class CliMethodExecutor<TValue> : MethodExecutorBase<string, TValue>, ICliFunctionExecutor<string, TValue, ParameterInfo> {

        public CliMethodExecutor(MethodInfo method, object? obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier, ICompositeValueConverter<TValue> valueConverter) : base(method, obj) {
            BindingFlags = bindingFlags;
            ClassMemberStringifier = classMemberStringifier;
            ValueConverter = valueConverter;
            AccessorsByNames = GetAccessorsByNames();
        }

        protected BindingFlags BindingFlags { get; }
        protected IClassMemberStringifier ClassMemberStringifier { get; }
        private IDictionary<string, ParameterInfo> AccessorsByNames { get; }
        private ICompositeValueConverter<TValue> ValueConverter { get; }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public override Type GetDataType(ParameterInfo accessor) {
            return ValueConverter.GetDataType(accessor.ParameterType);
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

        public bool IsCollection(string key) {
            ParameterInfo accessor = GetAccessor(key);
            return ValueConverter.IsCollection(accessor.ParameterType);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            return AccessorsByNames.TryGetValue(key, out result);
        }

        protected override TValue ConvertFromInternalValue(object? value, ParameterInfo accessor) {
            return ValueConverter.ConvertFrom(value, accessor.ParameterType);
        }

        protected override object? ConvertToInternalValue(TValue value, ParameterInfo accessor) {
            object? currentValue;
            try {
                currentValue = GetValueOrInitialize(accessor);
            } catch (MissingMethodException) {
                currentValue = null;
            }
            
            return ValueConverter.ConvertToOrUpdate(value, accessor.ParameterType, currentValue);
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

        public void SetParameterValues() {
            bool IsAssigned(int i) {
                return ParameterValues[i] != DBNull.Value;
            }

            int argumentIndex = 0;
            for (int parameterIndex = 0; parameterIndex < ParameterValues.Length; ++parameterIndex) {
                if (!IsAssigned(parameterIndex)) {
                    if (argumentIndex < Arguments.Count) {
                        object? value = ConvertToInternalValue(Arguments[argumentIndex++], ParameterInfos[parameterIndex]);
                        ParameterValues[parameterIndex] = value;
                    }
                }
            }
            if (argumentIndex < Arguments.Count) {
                throw CalqFramework.DataAccess.DataAccessErrors.UnexpectedArgument(Arguments[argumentIndex]);
            }
        }
    }
}
