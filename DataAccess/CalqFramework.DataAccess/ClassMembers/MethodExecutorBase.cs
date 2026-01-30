using System.Reflection;

namespace CalqFramework.DataAccess.ClassMembers;

public abstract class MethodExecutorBase<TParameterKey, TParameterValue> : ParameterStoreBase<TParameterKey, TParameterValue>, IFunctionExecutor<TParameterKey, TParameterValue> {

    protected MethodExecutorBase(MethodInfo method, object? obj) : base(method) {
        ParentObject = obj;
        Arguments = new List<TParameterValue>();
        for (int j = 0; j < ParameterIndexByParameter.Count; j++) {
            ParameterValues[j] = DBNull.Value;
        }
    }
    public object? ParentObject { get; }

    protected List<TParameterValue> Arguments { get; }

    public override object? this[ParameterInfo accessor] {
        get {
            var result = base[accessor];
            if (result != null && result is DBNull) {
                if (accessor.HasDefaultValue) {
                    result = accessor.DefaultValue;
                } else {
                    result = null;
                }
            }
            return result;
        }
        set {
            base[accessor] = value;
        }
    }
    public void AddArgument(TParameterValue value) {
        Arguments.Add(value);
    }

    public object? Invoke() {
        bool IsAssigned(int i) {
            return ParameterValues[i] == DBNull.Value;
        }

        int argumentIndex = 0;
        for (int parameterIndex = 0; parameterIndex < ParameterValues.Length; ++parameterIndex) {
            if (IsAssigned(parameterIndex)) {
                if (argumentIndex < Arguments.Count) {
                    object? value = ConvertToInternalValue(Arguments[argumentIndex++], ParameterInfos[parameterIndex]);
                    ParameterValues[parameterIndex] = value;
                } else {
                    ParameterValues[parameterIndex] = ParameterInfos[parameterIndex].HasDefaultValue ? ParameterInfos[parameterIndex].DefaultValue : throw DataAccessErrors.UnassignedParameter(ParameterInfos[parameterIndex].Name);
                }
            }
        }
        if (argumentIndex < Arguments.Count) {
            throw DataAccessErrors.UnexpectedArgument(Arguments[argumentIndex]);
        }
        return ParentMethod.Invoke(ParentObject, ParameterValues);
    }
}
