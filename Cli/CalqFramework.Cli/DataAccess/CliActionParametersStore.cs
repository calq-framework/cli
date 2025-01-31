using CalqFramework.Cli.Serialization.Parsing;
using CalqFramework.DataAccess;
using CalqFramework.DataAccess.ClassMember;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    internal class CliActionParametersStore : MethodParamStoreBase<string> {
        public List<string> PositionalParameters { get; }

        public CliActionParametersStore(MethodInfo method) : base(method) {
            PositionalParameters = new List<string>();
        }

        public override object? this[ParameterInfo accessor] {
            get {
                object? result = null;
                var type = GetDataType(accessor);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    result = base[accessor];
                } else {
                    var collection = (GetValueOrInitialize(accessor) as ICollection)!;
                    result = new CollectionStore(collection)["0"]; // FIXME return whole collection instead just first element. this is used for reading default value so print whole collection
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

        // TODO inherit from interface
        public string GetHelpString() {
            var result = "";

            result += "[DESCRIPTION]\n";
            result += ToStringHelper.GetMemberSummary(Method);
            result += "\n";

            result += "[POSITIONAL PARAMETERS]\n";
            foreach (var parameter in this.ParameterIndexByParameter.Keys) {
                result += $"{ParameterToString(parameter)} # {ToStringHelper.GetTypeName(parameter.ParameterType)} {GetDefaultValue(parameter)}\n";
            }

            return result;
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
            var positionalParametersIndex = 0;
            for (var i = 0; i < ParameterValues.Length; ++i) {
                if (ParameterValues[i] == DBNull.Value) {
                    if (positionalParametersIndex < PositionalParameters.Count) {
                        var value = ValueParser.ParseValue(PositionalParameters[positionalParametersIndex++], Parameters[i].ParameterType, Parameters[i].Name!);
                        ParameterValues[i] = value;
                    } else {
                        ParameterValues[i] = Parameters[i].HasDefaultValue ? Parameters[i].DefaultValue : throw new CliException($"unassigned parameter {ParameterToString(Parameters[i])}"); ;
                    }
                }
            }
            if (positionalParametersIndex < PositionalParameters.Count) {
                throw new CliException($"unexpected positional parameter {PositionalParameters[positionalParametersIndex]}");
            }
            return Method.Invoke(parentObj, this.ParameterValues);
        }

        public void SetNextParameter(string parameterValue) {
            PositionalParameters.Add(parameterValue);
        }
    }
}