﻿using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {

    public abstract class ParameterStoreBase<TKey, TValue> : KeyValueStoreBase<TKey, TValue, ParameterInfo, object?> {

        public ParameterStoreBase(MethodInfo method) {
            ParentMethod = method;
            var i = 0;
            Parameters = ParentMethod.GetParameters();
            ParameterIndexByParameter = Parameters.ToDictionary(x => x, x => i++);
            ParameterValues = new object?[ParameterIndexByParameter.Count];
            for (var j = 0; j < ParameterIndexByParameter.Count; j++) {
                ParameterValues[j] = DBNull.Value;
            }
        }

        public override IEnumerable<ParameterInfo> Accessors => Parameters.Where(ContainsAccessor);

        protected Dictionary<ParameterInfo, int> ParameterIndexByParameter { get; }

        protected ParameterInfo[] Parameters { get; }

        protected object?[] ParameterValues { get; }

        protected MethodInfo ParentMethod { get; }

        public override object? this[ParameterInfo accessor] {
            get {
                return ParameterValues[ParameterIndexByParameter[accessor]];
            }
            set {
                ParameterValues[ParameterIndexByParameter[accessor]] = value;
            }
        }

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
            return new MissingMemberException($"Missing {key} in {ParentMethod}.");
        }
    }
}