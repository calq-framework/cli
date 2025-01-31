﻿using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class MethodParamStoreBase<TKey> : KeyValueStoreBase<TKey, object?, ParameterInfo> {
        public MethodParamStoreBase(MethodInfo method) {
            Method = method;
            var i = 0;
            Parameters = Method.GetParameters();
            ParameterIndexByParameter = Parameters.ToDictionary(x => x, x => i++);
            ParameterValues = new object?[ParameterIndexByParameter.Count];
            for (var j = 0; j < ParameterIndexByParameter.Count; j++) {
                ParameterValues[j] = DBNull.Value;
            }
        }

        public override object? this[ParameterInfo accessor] {
            get {
                return ParameterValues[ParameterIndexByParameter[accessor]];
            }
            set {
                ParameterValues[ParameterIndexByParameter[accessor]] = value;
            }
        }

        public override IEnumerable<ParameterInfo> Accessors => ParameterIndexByParameter.Keys;

        protected MethodInfo Method { get; }
        public ParameterInfo[] Parameters { get; }
        protected Dictionary<ParameterInfo, int> ParameterIndexByParameter { get; }
        protected object?[] ParameterValues { get; }

        public override Type GetDataType(ParameterInfo accessor) {
            return accessor.ParameterType;
        }

        public override object? GetValueOrInitialize(ParameterInfo accessor) {
            var value = ParameterValues[ParameterIndexByParameter[accessor]];
            value = value != DBNull.Value ? value : null;
            value = value ??
                   Activator.CreateInstance(accessor.ParameterType) ??
                   Activator.CreateInstance(Nullable.GetUnderlyingType(accessor.ParameterType)!)!;
            ParameterValues[ParameterIndexByParameter[accessor]] = value;
            return value;
        }

        protected override MissingMemberException CreateMissingMemberException(TKey key) {
            return new MissingMemberException($"Missing {key} in {Method}.");
        }
    }
}