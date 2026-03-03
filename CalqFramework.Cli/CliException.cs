using System;

namespace CalqFramework.Cli {

    /// <summary>
    /// Exception thrown when CLI command parsing or execution fails.
    /// </summary>
    [Serializable]
    public class CliException : Exception {

        public CliException() {
        }

        public CliException(string? message) : base(message) {
        }

        public CliException(string? message, Exception? innerException) : base(message, innerException) {
        }

        /// <summary>
        /// Creates a CliException with formatted message for option errors.
        /// </summary>
        /// <param name="option">The option name</param>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public CliException(string option, string message, Exception innerException) 
            : base($"option '{option}': {message}", innerException) {
        }

        /// <summary>
        /// Creates a CliException with formatted message for option=value errors.
        /// </summary>
        /// <param name="option">The option name</param>
        /// <param name="value">The option value</param>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public CliException(string option, string value, string message, Exception innerException) 
            : base($"option '{option}={value}': {message}", innerException) {
        }
    }
}
