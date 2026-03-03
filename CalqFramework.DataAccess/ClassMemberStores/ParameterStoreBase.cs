using System.Reflection;
using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccess.ClassMemberStores;

public abstract class ParameterStoreBase<TKey, TValue> : KeyValueStoreBase<TKey, TValue, ParameterInfo, object?> {

    protected ParameterStoreBase(MethodInfo method) {
        ParentMethod = method;
        int i = 0;
        ParameterInfos = ParentMethod.GetParameters();
        ParameterIndexByParameter = ParameterInfos.ToDictionary(x => x, x => i++);
        ParameterValues = new object?[ParameterIndexByParameter.Count];
    }

    public override IEnumerable<ParameterInfo> Accessors => ParameterInfos.Where(ContainsAccessor);

    protected Dictionary<ParameterInfo, int> ParameterIndexByParameter { get; }

    protected ParameterInfo[] ParameterInfos { get; }

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

    public override Type GetValueType(ParameterInfo accessor) {
        return accessor.ParameterType;
    }

    public override object? GetValueOrInitialize(ParameterInfo accessor) {
        object? value = ParameterValues[ParameterIndexByParameter[accessor]];
        value = value != DBNull.Value ? value : null;
        value ??= accessor.ParameterType.CreateInstance();
        ParameterValues[ParameterIndexByParameter[accessor]] = value;
        return value;
    }
}
