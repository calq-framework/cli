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
    }
}
