using System.Collections.Generic;

namespace CalqFramework.Cli {

    /// <summary>
    /// Provides completion suggestions for CLI parameters and options.
    /// </summary>
    public interface ICompletionProvider {

        /// <summary>
        /// Gets completion suggestions based on the current partial input.
        /// </summary>
        /// <param name="partialInput">The partial input to complete (may be empty).</param>
        /// <returns>Collection of possible completions.</returns>
        IEnumerable<string> GetCompletions(string partialInput);
    }
}
