using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CalqFramework.DataAccess.ClassMember {
    public abstract class MethodExecutorBase<TParameterKey, TParameterValue> : ParameterStoreBase<TParameterKey, TParameterValue>, IFunctionExecutor<TParameterKey, TParameterValue> {
        protected MethodExecutorBase(MethodInfo method, object obj) : base(method) {
            ParentObject = obj;
        }

        public object ParentObject { get; }

        public abstract void AddParameter(TParameterValue value);

        public abstract object? Invoke();
    }
}
