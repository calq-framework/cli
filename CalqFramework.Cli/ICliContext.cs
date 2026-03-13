using CalqFramework.Cli.DataAccess;

namespace CalqFramework.Cli;

/// <summary>
///     Provides context and configuration for CLI operations.
/// </summary>
public interface ICliContext {
    /// <summary>
    ///     Factory for creating CLI component stores (options, subcommands, submodules).
    /// </summary>
    ICliComponentStoreFactory CliComponentStoreFactory { get; }

    /// <summary>
    ///     Skip unknown options instead of throwing an exception.
    /// </summary>
    bool SkipUnknown { get; }

    /// <summary>
    ///     TextWriter for interface description output (help, completions, framework messages).
    ///     Defaults to Console.Out if not specified.
    /// </summary>
    TextWriter InterfaceOut { get; }
}
