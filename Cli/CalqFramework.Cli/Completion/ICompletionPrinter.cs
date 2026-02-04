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
        void PrintSubmodules(IEnumerable<Submodule> submodules, string partialInput);

        /// <summary>
        /// Prints completion suggestions for subcommand keys.
        /// </summary>
        void PrintSubcommands(IEnumerable<Subcommand> subcommands, string partialInput);

        /// <summary>
        /// Prints completion suggestions for option keys.
        /// </summary>
        void PrintOptions(IEnumerable<Option> options, string partialInput);

        /// <summary>
        /// Prints completion suggestions for parameter keys.
        /// </summary>
        void PrintParameters(IEnumerable<Parameter> parameters, string partialInput);

        /// <summary>
        /// Prints completion suggestions for a submodule value.
        /// </summary>
        void PrintSubmoduleValue(Submodule submodule, string partialInput);

        /// <summary>
        /// Prints completion suggestions for a subcommand value.
        /// </summary>
        void PrintSubcommandValue(Subcommand subcommand, string partialInput);

        /// <summary>
        /// Prints completion suggestions for an option value.
        /// </summary>
        void PrintOptionValue(Option option, string partialInput);

        /// <summary>
        /// Prints completion suggestions for a parameter value.
        /// </summary>
        void PrintParameterValue(Parameter parameter, string partialInput);
    }
}
