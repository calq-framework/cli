using System.Reflection;

namespace CalqFramework.Cli.Formatting;

/// <summary>
///     Default implementation of <see cref="IVersionPrinter" /> that prints the assembly version.
/// </summary>
public class VersionPrinter : IVersionPrinter {
    /// <summary>
    ///     Include revision number in version output (4 digits instead of 3).
    /// </summary>
    public bool UseRevisionVersion { get; init; } = false;

    /// <inheritdoc />
    public void PrintVersion(ICliContext context, Type rootSubmoduleType) {
        var version = Assembly.GetEntryAssembly()?.GetName()
            .Version?.ToString(UseRevisionVersion ? 4 : 3);
        if (version != null) {
            context.InterfaceOut.WriteLine(version);
        }
    }
}
