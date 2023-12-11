using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class MethodParamsAccessor {
        public object Obj { get; }
        public ParameterInfo[] Parameters { get; }
        public object[] ParamValues { get; }
        public HashSet<ParameterInfo> AssignedParameters { get; }

        public MethodParamsAccessor(object obj, ParameterInfo[] parameters) {
            Obj = obj;
            Parameters = parameters;
            ParamValues = new object?[parameters.Length];
            AssignedParameters = new HashSet<ParameterInfo>();
        }

        private bool TryGetParamIndex(string dataMemberKey, out int result) {
            result = default;
            var success = false;

            if (dataMemberKey.Length == 1) {
                for (var i = 0; i < Parameters.Length; i++) { 
                    var param = Parameters[i];
                    if (param.Name == dataMemberKey) {
                        result = i;
                        success = true;
                    }
                }
            } else {
                for (var i = 0; i < Parameters.Length; i++) {
                    var param = Parameters[i];
                    if (param.Name == dataMemberKey) {
                        result = i;
                        success = true;
                    }
                }
            }

            return success;
        }

        public bool TryResolveDataMemberKey(string dataMemberKey, out string? result) {
            result = default;

            if (TryGetParamIndex(dataMemberKey, out var index)) {
                result = Parameters[index].Name!;
                return true;
            } else {
                return false;
            }
        }

        public bool TryGetDataType(string dataMemberKey, out Type? result) {
            result = default;

            if (TryGetParamIndex(dataMemberKey, out var index)) {
                result = Parameters[index].ParameterType;
                return true;
            } else {
                return false;
            }
        }

        //object? GetDataValue(string dataMemberKey);

        public bool TrySetDataValue(string dataMemberKey, object? value) {
            if (TryGetParamIndex(dataMemberKey, out var index)) {
                ParamValues[index] = value;
                AssignedParameters.Add(Parameters[index]);
                return true;
            } else {
                return false;
            }
        }

        public void SetDataValue(int dataMemberKey, object? value) {
            ParamValues[dataMemberKey] = value;
            AssignedParameters.Add(Parameters[dataMemberKey]);
        }
    }
}