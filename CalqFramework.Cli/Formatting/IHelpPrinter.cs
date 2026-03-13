using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Formatting;

/// <summary>
///     Prints formatted help information for CLI components to the console.
/// </summary>
public interface IHelpPrinter {
    /// <summary>
    ///     Prints help for a submodule showing its submodules, subcommands, and options.
    /// </summary>
    void PrintHelp(ICliContext context, Type rootType, Submodule rootSubmodule, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);

    /// <summary>
    ///     Prints help for the root type showing its submodules, subcommands, and options.
    /// </summary>
    void PrintHelp(ICliContext context, Type rootType, IEnumerable<Submodule> submodules, IEnumerable<Subcommand> subcommands, IEnumerable<Option> options);

    /// <summary>
    ///     Prints detailed help for a specific subcommand showing its parameters and options.
    /// </summary>
    void PrintSubcommandHelp(ICliContext context, Type rootType, Subcommand subcommand, IEnumerable<Option> options);
}
