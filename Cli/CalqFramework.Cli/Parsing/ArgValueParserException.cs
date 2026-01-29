using System;

namespace CalqFramework.Cli.Parsing;

/// <summary>
/// Exception thrown when CLI value parsing fails.
/// </summary>
public class ArgValueParserException : Exception {
    public ArgValueParserException(string message) : base(message) { }
    
    public ArgValueParserException(string message, Exception innerException) : base(message, innerException) { }
}
