namespace CalqFramework.DataAccess {

    /// <summary>
    /// Provides a key-value store that can execute functions (methods) with parameters.
    /// </summary>
    public interface IFunctionExecutor<TParameterKey, TParameterValue> : IKeyValueStore<TParameterKey, TParameterValue> {

        /// <summary>
        /// Adds a positional argument value to the function parameters.
        /// </summary>
        void AddArgument(TParameterValue value);

        /// <summary>
        /// Invokes the function with the configured parameters and returns the result.
        /// </summary>
        object? Invoke();
    }
}
