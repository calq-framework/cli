namespace CalqFramework.DataAccess {

    public interface IFunctionExecutor<TParameterKey, TParameterValue> : IKeyValueStore<TParameterKey, TParameterValue> {

        void AddArgument(TParameterValue value);

        object? Invoke();
    }
}
