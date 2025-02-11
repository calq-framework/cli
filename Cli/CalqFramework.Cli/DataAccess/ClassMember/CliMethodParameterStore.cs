using CalqFramework.Cli.Parsing;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess.ClassMember {
    internal class CliMethodParameterStore : MethodParamStoreBase<string, string?>, ICliStore<string, string?, ParameterInfo> {
        public List<string> ReceivedPositionalParameters { get; }

        public CliMethodParameterStore(MethodInfo method) : base(method) {
            ReceivedPositionalParameters = new List<string>();
        }

        public override object? this[ParameterInfo accessor] {
            get {
                object? result = null;
                var type = GetDataType(accessor);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    result = base[accessor];
                    if (result is DBNull && accessor.HasDefaultValue) {
                        result = accessor.DefaultValue;
                    }
                } else {
                    var collection = (GetValueOrInitialize(accessor) as ICollection)!;
                    if (collection.Count > 0) {
                        result = new CollectionStore(collection)["0"]; // FIXME return whole collection instead just first element. this is used for reading default value so print whole collection
                    }
                }
                return result;
            }
            set {
                var type = GetDataType(accessor);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    base[accessor] = value;
                } else {
                    var collection = (GetValueOrInitialize(accessor) as ICollection)!;
                    new CollectionStore(collection).AddValue(value);
                }
            }
        }

        // TODO move this logic into serializer
        public static string? GetDefaultValue(ParameterInfo parameter) => parameter.HasDefaultValue ? $"({parameter.DefaultValue?.ToString()!.ToLower()})" : "";

        public static string ParameterToString(ParameterInfo parameterInfo) {
            return parameterInfo.Name!;
        }

        public override bool ContainsAccessor(ParameterInfo accessor) {
            return ParameterIndexByParameter.Keys.Contains(accessor);
        }

        public override bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            result = default;
            var success = false;

            if (key.Length == 1) {
                foreach (var parameter in ParameterIndexByParameter.Keys) {
                    if (parameter.Name[0] == key[0]) {
                        result = parameter;
                        success = true;
                    }
                }
            } else {
                foreach (var parameter in ParameterIndexByParameter.Keys) {
                    if (parameter.Name == key) {
                        result = parameter;
                        success = true;
                    }
                }
            }

            return success;
        }

        internal object? Invoke(object parentObj) {
            var receivedPositionalParametersIndex = 0;
            for (var i = 0; i < ParameterValues.Length; ++i) {
                if (ParameterValues[i] == DBNull.Value) {
                    if (receivedPositionalParametersIndex < ReceivedPositionalParameters.Count) {
                        var value = ValueParser.ParseValue(ReceivedPositionalParameters[receivedPositionalParametersIndex++], Parameters[i].ParameterType, Parameters[i].Name!);
                        ParameterValues[i] = value;
                    } else {
                        ParameterValues[i] = Parameters[i].HasDefaultValue ? Parameters[i].DefaultValue : throw new CliException($"unassigned parameter {ParameterToString(Parameters[i])}"); ;
                    }
                }
            }
            if (receivedPositionalParametersIndex < ReceivedPositionalParameters.Count) {
                throw new CliException($"unexpected positional parameter {ReceivedPositionalParameters[receivedPositionalParametersIndex]}");
            }
            return Method.Invoke(parentObj, ParameterValues);
        }

        public void AddPositionalParameter(string parameterValue) {
            ReceivedPositionalParameters.Add(parameterValue);
        }

        public IDictionary<ParameterInfo, IEnumerable<string>> GetKeysByAccessors() {
            var keys = new Dictionary<ParameterInfo, IEnumerable<string>>();
            foreach (var accessor in Accessors) {
                var accesorKeys = new List<string>();
                //foreach (var atribute in accessor.GetCustomAttributes<NameAttribute>()) {
                //    accesorKeys.Add(atribute.Name);
                //}
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

        protected override string? ConvertFromInternalValue(object? value, ParameterInfo accessor) {
            return value?.ToString()?.ToLower();
        }

        protected override object? ConvertToInternalValue(string? value, ParameterInfo accessor) {
            return ValueParser.ParseValue(value, GetDataType(accessor));
        }
    }
}