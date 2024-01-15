using CalqFramework.Serialization.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options.DataMemberAccess {
    public class MethodParamsAccessor : DataAccessorBase, IDataAccessor {

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

        private bool TryGetParamIndex(string key, out int result) {
            result = default;
            var success = false;

            if (key.Length == 1) {
                for (var i = 0; i < Parameters.Length; i++) { 
                    var param = Parameters[i];
                    if (param.Name == key) {
                        result = i;
                        success = true;
                    }
                }
            } else {
                for (var i = 0; i < Parameters.Length; i++) {
                    var param = Parameters[i];
                    if (param.Name == key) {
                        result = i;
                        success = true;
                    }
                }
            }

            return success;
        }

        public bool TryResolveDataMemberKey(string key, out string? result) {
            result = default;

            if (TryGetParamIndex(key, out var index)) {
                result = Parameters[index].Name!;
                return true;
            } else {
                return false;
            }
        }

        public bool TryGetDataType(string key, out Type? result) {
            result = default;

            if (TryGetParamIndex(key, out var index)) {
                result = Parameters[index].ParameterType;
                return true;
            } else {
                return false;
            }
        }

        //object? GetDataValue(string key);

        // TODO unify collection handling logic with DataMemberAndMethodParamsAccessor
        public bool TrySetDataValue(string key, object? value) {
            if (TryGetParamIndex(key, out var index)) {
                TryGetDataType(key, out var type);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    ParamValues[index] = value;
                } else {
                    var collection = ParamValues[index];
                    if (collection == null) {
                        collection = Activator.CreateInstance(type);
                        ParamValues[index] = collection;
                    }
                    CollectionAccessor.AddValue((collection as ICollection)!, value);
                }

                AssignedParameters.Add(Parameters[index]);
                return true;
            } else {
                return false;
            }
        }

        public string ResolveDataMemberKey(int key) {
            if (key >= Parameters.Length) {
                throw new Exception("passed too many args");
            }
            return Parameters[key].Name!;
        }

        public Type GetParameterType(int index) {
            if (index >= Parameters.Length) {
                throw new Exception("passed too many args");
            }
            return Parameters[index].ParameterType;
        }

        public void SetDataValue(int key, object? value) {
            ParamValues[key] = value;
            AssignedParameters.Add(Parameters[key]);
        }

        internal object? Invoke() {
            return Method.Invoke(TargetObj, ParamValues);
        }

        public override object GetOrInitializeValue(string key) {
            if (TryGetParamIndex(key, out var index)) {
                var value = ParamValues[index] ??
                   Activator.CreateInstance(Parameters[index].ParameterType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(Parameters[index].ParameterType)!)!;
                ParamValues[index] = value;
                return value;
            }
            throw new MissingMemberException();
        }

        public override Type GetType(string key) {
            if (TryGetParamIndex(key, out var index)) {
                return Parameters[index].ParameterType;
            }
            throw new MissingMemberException();
        }

        public override object? GetValue(string key) {
            if (TryGetParamIndex(key, out var index)) {
                return ParamValues[index];
            }
            throw new MissingMemberException();
        }

        public override bool HasKey(string key) {
            return TryGetParamIndex(key, out var _);
        }

        public override void SetValue(string key, object? value) {
            if (TryGetParamIndex(key, out var index)) {
                ParamValues[index] = value;
                AssignedParameters.Add(Parameters[index]);
                return;
            }
            throw new MissingMemberException();
        }

        public override bool SetOrAddValue(string key, object? value) {
            var type = GetType(key);
            var isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                SetValue(key, value);
                return false;
            } else {
                var collectionObj = (GetOrInitializeValue(key) as ICollection)!;
                AddValue(collectionObj, value);
                TryGetParamIndex(key, out var index);
                AssignedParameters.Add(Parameters[index]);
                return true;
            }
        }
    }
}