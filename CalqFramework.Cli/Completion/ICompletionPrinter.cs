using System.Collections.Generic;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Interface for printing completion suggestions.
    /// </summary>
    public interface ICompletionPrinter {

        /// <summary>
        /// Prints completion suggestions for submodule keys.
        /// </summary>
        void PrintSubmodules(ICliContext context, IEnumerable<Submodule> submodules, string partialInput);

        /// <summary>
        /// Prints completion suggestions for subcommand keys.
        /// </summary>
        void PrintSubcommands(ICliContext context, IEnumerable<Subcommand> subcommands, string partialInput);

        /// <summary>
        /// Prints completion suggestions for both parameter and option keys (deduplicated).
        /// </summary>
        void PrintParametersAndOptions(ICliContext context, IEnumerable<Parameter> parameters, IEnumerable<Option> options, string partialInput);

        /// <summary>
        /// Prints completion suggestions for a submodule value.
        /// </summary>
        void PrintSubmoduleValue(ICliContext context, Submodule submodule, string partialInput);

        /// <summary>
        /// Prints completion suggestions for a subcommand value.
        /// </summary>
        void PrintSubcommandValue(ICliContext context, Subcommand subcommand, string partialInput);

        /// <summary>
        /// Prints completion suggestions for an option value.
        /// </summary>
        void PrintOptionValue(ICliContext context, Option option, string partialInput, object? submodule);

        /// <summary>
        /// Prints completion suggestions for a parameter value.
        /// </summary>
        void PrintParameterValue(ICliContext context, Parameter parameter, string partialInput, object? submodule);
    }
}
