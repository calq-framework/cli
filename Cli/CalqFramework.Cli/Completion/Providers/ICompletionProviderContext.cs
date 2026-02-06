namespace CalqFramework.Cli.Completion.Providers {

    /// <summary>
    /// Context information for completion providers.
    /// </summary>
    public interface ICompletionProviderContext {
        
        /// <summary>
        /// The partial input being completed.
        /// </summary>
        string PartialInput { get; }
        
        /// <summary>
        /// The submodule instance (for accessing state in method-based completions).
        /// </summary>
        object? Submodule { get; }
        
        /// <summary>
        /// Filter/method name from CliCompletionAttribute (for method-based completions).
        /// </summary>
        string? Filter { get; }
    }
}
