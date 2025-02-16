using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Parsing;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;

namespace CalqFramework.Cli.DataAccess.ClassMember {

    public class MethodExecutor : MethodExecutorBase<string, string?>, ICliFunctionExecutor<string, string?, ParameterInfo> {

        public MethodExecutor(MethodInfo method, object? obj = null) : base(method, obj) {
            ReceivedPositionalParameters = new List<string?>();
        }

        public List<string?> ReceivedPositionalParameters { get; }

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

        public override void AddParameter(string? value) {
            ReceivedPositionalParameters.Add(value);
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return accessor.Member == ParentMethod;
        }

        public IDictionary<ParameterInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<ParameterInfo, IEnumerable<string>>();
            foreach (ParameterInfo accessor in Accessors) {
                var accesorKeys = new List<string>();
                //foreach (var atribute in accessor.GetCustomAttributes<NameAttribute>()) {
                //    accesorKeys.Add(atribute.Name);
                //}
                if (accesorKeys.Count == 0) {
                    accesorKeys.Add(accessor.Name);
                }
                if (!accesorKeys.Select(x => x.Length == 1).Any()) {
                    accesorKeys.Add(accesorKeys[0][0].ToString());
                }
                keys[accessor] = accesorKeys;
            }
            return keys;
        }

        public override object? Invoke() {
            int receivedPositionalParametersIndex = 0;
            for (int i = 0; i < ParameterValues.Length; ++i) {
                if (ParameterValues[i] == DBNull.Value) {
                    if (receivedPositionalParametersIndex < ReceivedPositionalParameters.Count) {
                        object value = ValueParser.ParseValue(ReceivedPositionalParameters[receivedPositionalParametersIndex++], Parameters[i].ParameterType, Parameters[i].Name!);
                        ParameterValues[i] = value;
                    } else {
                        ParameterValues[i] = Parameters[i].HasDefaultValue ? Parameters[i].DefaultValue : throw new CliException($"unassigned parameter {Parameters[i].Name}"); ;
                    }
                }
            }
            if (receivedPositionalParametersIndex < ReceivedPositionalParameters.Count) {
                throw new CliException($"unexpected positional parameter {ReceivedPositionalParameters[receivedPositionalParametersIndex]}");
            }
            return ParentMethod.Invoke(ParentObject, ParameterValues);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = default;
            bool success = false;

            if (key.Length == 1) {
                foreach (ParameterInfo parameter in ParameterIndexByParameter.Keys) {
                    if (parameter.Name[0] == key[0]) {
                        result = parameter;
                        success = true;
                    }
                }
            } else {
                foreach (ParameterInfo parameter in ParameterIndexByParameter.Keys) {
                    if (parameter.Name == key) {
                        result = parameter;
                        success = true;
                    }
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
