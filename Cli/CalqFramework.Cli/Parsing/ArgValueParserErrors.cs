using System;

namespace CalqFramework.Cli.Parsing {

    /// <summary>
    /// Factory class for creating common ArgValueParserException instances with consistent messaging.
    /// </summary>
    public static class ArgValueParserErrors {

        /// <summary>
        /// Creates an exception for out of range values.
        /// </summary>
        public static ArgValueParserException OutOfRange(object? min, object? max, Exception innerException) =>
            new ArgValueParserException($"Out of range ({min}-{max})", innerException);

        /// <summary>
        /// Creates an exception for invalid format.
        /// </summary>
        public static ArgValueParserException InvalidFormat(string expectedTypeName, Exception innerException) =>
            new ArgValueParserException($"Invalid format (expected {expectedTypeName})", innerException);
    }
}
