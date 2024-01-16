using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.DataAccess
{
    internal class MethodParamAccessor : DataAccessorBase
    {

        public ParameterInfo[] Parameters { get; }
        private object?[] ParamValues { get; }
        public HashSet<ParameterInfo> AssignedParameters { get; }
        public object TargetObj { get; }
        public string MethodName { get; }
        public BindingFlags BindingAttr { get; }
        public MethodInfo Method { get; }

        public MethodParamAccessor(object targetObj, string methodName, BindingFlags methodBindingAttr)
        {
            TargetObj = targetObj;
            MethodName = methodName;
            BindingAttr = methodBindingAttr;

            Method = ResolveMethod(targetObj, MethodName, BindingAttr);
            Parameters = Method.GetParameters();
            ParamValues = new object?[Parameters.Length];
            AssignedParameters = new HashSet<ParameterInfo>();
        }

        private MethodInfo ResolveMethod(object targetObject, string action, BindingFlags bindingAttr)
        {
            var method = targetObject.GetType().GetMethod(action, bindingAttr);
            if (method == null)
            {
                throw new CliException($"invalid command");
            }
            return method;
        }

        public object? Invoke()
        {
            var assignedParamNames = AssignedParameters.Select(x => x.Name).ToHashSet();
            for (var j = 0; j < Parameters.Length; ++j)
            {
                var param = Parameters[j];
                if (!assignedParamNames.Contains(param.Name))
                {
                    if (!param.IsOptional)
                    {
                        throw new CliException($"unassigned option {param.Name}");
                    }
                    SetValue(j, param.DefaultValue!);
                }
            }
            return Method.Invoke(TargetObj, ParamValues);
        }

        private bool TryGetParamIndex(string key, out int result)
        {
            result = default;
            var success = false;

            if (key.Length == 1)
            {
                for (var i = 0; i < Parameters.Length; i++)
                {
                    var param = Parameters[i];
                    if (param.Name == key)
                    {
                        result = i;
                        success = true;
                    }
                }
            }
            else
            {
                for (var i = 0; i < Parameters.Length; i++)
                {
                    var param = Parameters[i];
                    if (param.Name == key)
                    {
                        result = i;
                        success = true;
                    }
                }
            }

            return success;
        }

        public string GetKey(int index)
        {
            if (index >= Parameters.Length)
            {
                throw new CliException("passed too many args");
            }
            return Parameters[index].Name!;
        }

        public Type GetType(int index)
        {
            if (index >= Parameters.Length)
            {
                throw new CliException("passed too many args");
            }
            return Parameters[index].ParameterType;
        }

        public void SetValue(int index, object? value)
        {
            if (index >= Parameters.Length)
            {
                throw new CliException("passed too many args");
            }
            ParamValues[index] = value;
            AssignedParameters.Add(Parameters[index]);
        }

        public override object GetOrInitializeValue(string key)
        {
            if (TryGetParamIndex(key, out var index))
            {
                var value = ParamValues[index] ??
                   Activator.CreateInstance(Parameters[index].ParameterType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(Parameters[index].ParameterType)!)!;
                ParamValues[index] = value;
                return value;
            }
            throw new MissingMemberException();
        }

        public override Type GetType(string key)
        {
            if (TryGetParamIndex(key, out var index))
            {
                return Parameters[index].ParameterType;
            }
            throw new MissingMemberException();
        }

        public override object? GetValue(string key)
        {
            if (TryGetParamIndex(key, out var index))
            {
                return ParamValues[index];
            }
            throw new MissingMemberException();
        }

        public override bool HasKey(string key)
        {
            return TryGetParamIndex(key, out var _);
        }

        public override void SetValue(string key, object? value)
        {
            if (TryGetParamIndex(key, out var index))
            {
                ParamValues[index] = value;
                AssignedParameters.Add(Parameters[index]);
                return;
            }
            throw new MissingMemberException();
        }

        public override bool SetOrAddValue(string key, object? value)
        {
            var type = GetType(key);
            var isCollection = type.GetInterface(nameof(ICollection)) != null;
            if (isCollection == false)
            {
                SetValue(key, value);
                return false;
            }
            else
            {
                var collectionObj = (GetOrInitializeValue(key) as ICollection)!;
                AddValue(collectionObj, value);
                TryGetParamIndex(key, out var index);
                AssignedParameters.Add(Parameters[index]);
                return true;
            }
        }
    }
}