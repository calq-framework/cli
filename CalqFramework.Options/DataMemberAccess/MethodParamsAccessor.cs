using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class MethodParamsAccessor {

        public ParameterInfo[] Parameters { get; }
        private object[] ParamValues { get; }
        public HashSet<ParameterInfo> AssignedParameters { get; }
        public object TargetObj { get; }
        public string Action { get; }
        public BindingFlags BindingAttr { get; }
        public MethodInfo Method { get; }

        private MethodInfo ResolveMethod(object targetObject, string action, BindingFlags bindingAttr) {
            var method = targetObject.GetType().GetMethod(action, bindingAttr);
            if (method == null) {
                throw new Exception($"invalid command");
            }
            return method;
        }

        public MethodParamsAccessor(object targetObj, string action, BindingFlags bindingAttr) {
            TargetObj = targetObj;
            Action = action;
            BindingAttr = bindingAttr;

            Method = ResolveMethod(targetObj, Action, BindingAttr); 
            Parameters = Method.GetParameters();
            ParamValues = new object?[Parameters.Length];
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

        public string ResolveDataMemberKey(int dataMemberKey) {
            if (dataMemberKey >= Parameters.Length) {
                throw new Exception("passed too many args");
            }
            return Parameters[dataMemberKey].Name!;
        }

        public Type GetParameterType(int dataMemberKey) {
            if (dataMemberKey >= Parameters.Length) {
                throw new Exception("passed too many args");
            }
            return Parameters[dataMemberKey].ParameterType;
        }

        public void SetDataValue(int dataMemberKey, object? value) {
            ParamValues[dataMemberKey] = value;
            AssignedParameters.Add(Parameters[dataMemberKey]);
        }

        internal object? Invoke() {
            return Method.Invoke(TargetObj, ParamValues);
        }
    }
}