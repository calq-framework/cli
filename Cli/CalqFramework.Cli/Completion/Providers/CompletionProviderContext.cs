namespace CalqFramework.Cli.Completion.Providers {

    /// <summary>
    /// Implementation of completion provider context.
    /// </summary>
    internal class CompletionProviderContext : ICompletionProviderContext {
        
        public required string PartialInput { get; init; }
        public object? Submodule { get; init; }
        public string? Filter { get; init; }
    }
}
