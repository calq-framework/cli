namespace CalqFramework.DataAccess {

    public interface IFunctionExecutor<TParameterKey, TParameterValue> : IKeyValueStore<TParameterKey, TParameterValue> {
        void AddParameter(TParameterValue value);
        object? Invoke();
    }
}