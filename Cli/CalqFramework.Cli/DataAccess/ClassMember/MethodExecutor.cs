using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Parsing;
using CalqFramework.Cli.Serialization;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    public class MethodExecutor : MethodExecutorBase<string, string?>, ICliFunctionExecutor<string, string?, ParameterInfo> {

        public MethodExecutor(MethodInfo method, object? obj, BindingFlags bindingFlags, IClassMemberStringifier classMemberStringifier) : base(method, obj) {
            BindingFlags = bindingFlags;
            ClassMemberStringifier = classMemberStringifier;
        }

        protected BindingFlags BindingFlags { get; }
        protected IClassMemberStringifier ClassMemberStringifier { get; }

        public override object? this[ParameterInfo accessor] {
            get {
                object? result = null;
                Type type = GetDataType(accessor);
                bool isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    result = base[accessor];
                    if (result is DBNull && accessor.HasDefaultValue) {
                        result = accessor.DefaultValue;
                    }
                } else {
                    ICollection collection = (GetValueOrInitialize(accessor) as ICollection)!;
                    if (collection.Count > 0) {
                        result = new CollectionStore(collection)["0"]; // FIXME return whole collection instead just first element. this is used for reading default value so print whole collection
                    }
                }
                return result;
            }
            set {
                Type type = GetDataType(accessor);
                bool isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    base[accessor] = value;
                } else {
                    ICollection collection = (GetValueOrInitialize(accessor) as ICollection)!;
                    new CollectionStore(collection).AddValue(value);
                }
            }
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public IDictionary<ParameterInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<ParameterInfo, IEnumerable<string>>();
            foreach (ParameterInfo accessor in Accessors) {
                keys[accessor] = ClassMemberStringifier.GetNames(accessor);
            }
            return keys;
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = default;
            bool success = false;

            StringComparison stringComparison = BindingFlags.HasFlag(BindingFlags.IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (ParameterInfo parameter in ParameterIndexByParameter.Keys) {
                if (ClassMemberStringifier.GetNames(parameter).Where(x => string.Equals(x, key, stringComparison)).Any()) {
                    result = parameter;
                    success = true;
                }
            }

            return success;
        }

        protected override string? ConvertFromInternalValue(object? value, ParameterInfo accessor) {
            return value?.ToString()?.ToLower();
        }

        protected override object? ConvertToInternalValue(string? value, ParameterInfo accessor) {
            return ValueParser.ParseValue(value, GetDataType(accessor));
        }
    }
}
