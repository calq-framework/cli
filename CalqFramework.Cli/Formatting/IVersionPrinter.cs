namespace CalqFramework.Cli.Formatting;

/// <summary>
///     Prints version information for the CLI application.
/// </summary>
public interface IVersionPrinter {
    /// <summary>
    ///     Prints the version of the CLI application.
    /// </summary>
    void PrintVersion(ICliContext context, Type rootSubmoduleType);
}
