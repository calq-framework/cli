using System.Collections.Generic;

namespace CalqFramework.Cli.Completion.Providers {

    /// <summary>
    /// Provides completion suggestions for CLI parameters and options.
    /// </summary>
    public interface ICompletionProvider {

        /// <summary>
        /// Gets completion suggestions based on the completion context.
        /// </summary>
        /// <param name="context">The completion context containing partial input, submodule, and filter.</param>
        /// <returns>Collection of possible completions.</returns>
        IEnumerable<string> GetCompletions(ICompletionProviderContext context);
    }
}
