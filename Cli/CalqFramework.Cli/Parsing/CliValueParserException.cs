using System;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Exception thrown when CLI value parsing fails.
/// </summary>
public class CliValueParserException : Exception {
    public CliValueParserException(string message) : base(message) { }
    
    public CliValueParserException(string message, Exception innerException) : base(message, innerException) { }
}
