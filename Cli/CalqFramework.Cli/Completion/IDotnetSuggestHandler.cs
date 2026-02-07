using System.Collections.Generic;

namespace CalqFramework.Cli.Completion {

    public interface IDotnetSuggestHandler {
        
        /// <summary>
        /// Handles dotnet-suggest completion protocol.
        /// Converts [suggest] or [suggest:N] format to __complete format and delegates to CompletionHandler.
        /// </summary>
        ResultVoid HandleDotnetSuggest(ICliContext context, ICompletionHandler completionHandler, IEnumerable<string> args, object target);
        
        /// <summary>
        /// Registers the current executable with dotnet-suggest.
        /// </summary>
        void Register(string? commandPath = null);
        
        /// <summary>
        /// Unregisters the current executable from dotnet-suggest.
        /// </summary>
        void Unregister(string? commandPath = null);
    }
}
