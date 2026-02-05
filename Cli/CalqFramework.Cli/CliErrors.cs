using System;

namespace CalqFramework.Cli {

    /// <summary>
    /// Factory class for creating common CliException instances with consistent messaging.
    /// </summary>
    public static class CliErrors {

        /// <summary>
        /// Creates an exception for when an argument is not recognized as an option.
        /// </summary>
        public static CliException NotAnOption(string option) =>
            new CliException($"Not an option: {option}");

        /// <summary>
        /// Creates an exception for when an option is not recognized.
        /// </summary>
        public static CliException UnknownOption(string option) =>
            new CliException($"Unknown option: {option}");

        /// <summary>
        /// Creates an exception for when an option requires a value but none was provided.
        /// </summary>
        public static CliException OptionRequiresValue(string option) =>
            new CliException($"Option '{option}' requires a value");

        /// <summary>
        /// Creates an exception for when a value is unexpected.
        /// </summary>
        public static CliException UnexpectedValue(string value) =>
            new CliException($"Unexpected value: {value}");

        /// <summary>
        /// Creates an exception for ambiguous syntax around an option.
        /// </summary>
        public static CliException AmbiguousSyntax(string option) =>
            new CliException($"Ambiguous syntax around '{option}' (try using --)");

        /// <summary>
        /// Creates an exception for ambiguous values that could be options.
        /// </summary>
        public static CliException AmbiguousValue(string value, string option) =>
            new CliException($"Ambiguous value '{value}' for '{option}', use option=value format for values starting with '-' or '+'");

        /// <summary>
        /// Creates an exception for when a command is invalid.
        /// </summary>
        public static CliException InvalidCommand(string? command) =>
            new CliException($"Invalid command: {command}");

        /// <summary>
        /// Creates an exception for CLI name collisions.
        /// </summary>
        public static CliException NameCollision(string name1, string name2) =>
            new CliException($"CLI name of '{name1}' collides with '{name2}'");

        /// <summary>
        /// Creates an exception wrapping an argument parsing failure.
        /// </summary>
        public static CliException FailedToParseArgument(string message, Exception innerException) =>
            new CliException($"Failed to parse argument: {message}", innerException);

        /// <summary>
        /// Creates an exception wrapping a data access failure.
        /// </summary>
        public static CliException FailedToAccessData(string message, Exception innerException) =>
            new CliException($"Failed to access data: {message}", innerException);

        /// <summary>
        /// Creates an exception for option parsing errors with inner exception.
        /// </summary>
        public static CliException OptionError(string option, string message, Exception innerException) =>
            new CliException(option, message, innerException);

        /// <summary>
        /// Creates an exception for option=value parsing errors with inner exception.
        /// </summary>
        public static CliException OptionValueError(string option, string value, string message, Exception innerException) =>
            new CliException(option, value, message, innerException);

        /// <summary>
        /// Creates an exception for empty argument strings.
        /// </summary>
        public static CliException EmptyArgument() =>
            new CliException("Argument cannot be empty");

        /// <summary>
        /// Creates an exception for unsupported member types.
        /// </summary>
        public static CliException UnsupportedMemberType(string memberTypeName) =>
            new CliException($"Unsupported member type: {memberTypeName}");

        /// <summary>
        /// Creates an exception for invalid completion provider types.
        /// </summary>
        public static CliException InvalidCompletionProvider(string providerTypeName) =>
            new CliException($"Provider type '{providerTypeName}' must implement {nameof(ICompletionProvider)}");

        /// <summary>
        /// Creates an exception for unsupported shell types.
        /// </summary>
        public static CliException UnsupportedShell(string shell) =>
            new CliException($"Unsupported shell: {shell}. Supported shells: bash, zsh, powershell, fish");

        public static CliException UnknownCompletionSubcommand(string subcommand) =>
            new CliException($"Unknown completion subcommand: {subcommand}. Valid subcommands: complete, script, install, uninstall");

        public static CliException CompletionInstallFailed(string shell, string message, Exception innerException) =>
            new CliException($"Failed to install completion script for {shell}: {message}", innerException);

        /// <summary>
        /// Creates an exception for completion uninstallation failures.
        /// </summary>
        public static CliException CompletionUninstallFailed(string shell, string message, Exception innerException) =>
            new CliException($"Failed to uninstall completion script for {shell}: {message}", innerException);

        /// <summary>
        /// Creates an exception for when the program name cannot be determined.
        /// </summary>
        public static CliException UnableToDetermineProgramName() =>
            new CliException("Unable to determine program name from entry assembly");
    }
}
