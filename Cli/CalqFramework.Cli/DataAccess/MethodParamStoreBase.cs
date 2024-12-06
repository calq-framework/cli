using CalqFramework.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess {
    // TODO abstract
    // FIXME fix and move to DataAccess project
    public class MethodParamStoreBase : IKeyValueStore<string, object?, ParameterInfo> {
        protected ParameterInfo[] Parameters { get; }
        protected object?[] ParamValues { get; }
        protected HashSet<ParameterInfo> AssignedParameters { get; }
        protected MethodInfo Method { get; }
        protected BindingFlags BindingAttr { get; }

        public IEnumerable<ParameterInfo> Accessors => Parameters;

        public object? this[ParameterInfo accessor] { get => this[accessor.Name!]; set => this[accessor.Name!] = value; }

        public object? this[string key] {
            get {
                if (TryGetParamIndex(key, out var index)) {
                    return ParamValues[index];
                }
                throw new MissingMemberException();
            }
            set {
                // FIXME
                var type = GetDataType(key);
                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection == false) {
                    if (TryGetParamIndex(key, out var index)) {
                        SetValue(index, value);
                    }
                } else {
                    var collectionObj = (GetValueOrInitialize(key) as ICollection)!;
                    new CollectionStore(collectionObj).AddValue(value);
                    TryGetParamIndex(key, out var index);
                    AssignedParameters.Add(Parameters[index]);
                }
            }
        }

        public MethodParamStoreBase(MethodInfo method, BindingFlags bindingAttr) {
            Method = method;
            BindingAttr = bindingAttr;
            Parameters = Method.GetParameters();
            ParamValues = new object?[Parameters.Length];
            AssignedParameters = new HashSet<ParameterInfo>();
        }

        public object? Invoke(object? obj) {
            var assignedParamNames = AssignedParameters.Select(x => x.Name).ToHashSet();
            for (var j = 0; j < Parameters.Length; ++j) {
                var param = Parameters[j];
                if (!assignedParamNames.Contains(param.Name)) {
                    if (!param.IsOptional) {
                        throw new CliException($"unassigned parameter {param.Name}");
                    }
                    SetValue(j, param.DefaultValue!);
                }
            }
            return Method.Invoke(obj, ParamValues);
        }

        private bool TryGetParamIndex(string key, out int result) {
            result = default;
            var success = false;

            if (key.Length == 1) {
                for (var i = 0; i < Parameters.Length; i++) {
                    var param = Parameters[i];
                    if (param.Name[0] == key[0]) {
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

        public string GetKey(int index) {
            if (index >= Parameters.Length) {
                throw new CliException("passed too many args");
            }
            return Parameters[index].Name!;
        }

        public Type GetType(int index) {
            if (index >= Parameters.Length) {
                throw new CliException("passed too many args");
            }
            return Parameters[index].ParameterType;
        }

        public void SetValue(int index, object? value) {
            if (index >= Parameters.Length) {
                throw new CliException("passed too many args");
            }
            ParamValues[index] = value;
            AssignedParameters.Add(Parameters[index]);
        }

        public object GetValueOrInitialize(string key) {
            if (TryGetParamIndex(key, out var index)) {
                var value = ParamValues[index] ??
                   Activator.CreateInstance(Parameters[index].ParameterType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(Parameters[index].ParameterType)!)!;
                ParamValues[index] = value;
                return value;
            }
            throw new MissingMemberException();
        }

        public Type GetDataType(string key) {
            if (TryGetParamIndex(key, out var index)) {
                return Parameters[index].ParameterType;
            }
            throw new MissingMemberException();
        }

        public bool ContainsKey(string key) {
            return TryGetParamIndex(key, out var _);
        }

        public bool SetOrAddValue(string key, object? value) {
            var type = GetDataType(key);
            var isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false) {
                this[key] = value;
                return false;
            } else {
                var collectionObj = (GetValueOrInitialize(key) as ICollection)!;
                new CollectionStore(collectionObj).AddValue(value);
                TryGetParamIndex(key, out var index);
                AssignedParameters.Add(Parameters[index]);
                return true;
            }
        }

        public bool TryGetAccessor(string key, [MaybeNullWhen(false)] out ParameterInfo result) {
            var found = TryGetParamIndex(key, out int index);
            if (found) {
                result = Parameters[index];
            } else {
                result = null;
            }
            return found;
        }

        public ParameterInfo GetAccessor(string key) {
            throw new NotImplementedException();
        }

        public bool ContainsAccessor(ParameterInfo key) {
            throw new NotImplementedException();
        }

        public string AccessorToString(ParameterInfo accessor) {
            return accessor.Name;
        }

        public Type GetDataType(ParameterInfo accessor) {
            return GetDataType(accessor.Name!);
        }

        public object? GetValueOrInitialize(ParameterInfo accessor) {
            throw new NotImplementedException();
        }
    }
}