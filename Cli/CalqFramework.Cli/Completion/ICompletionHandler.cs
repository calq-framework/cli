using System.Collections.Generic;

namespace CalqFramework.Cli.Completion {

    public interface ICompletionHandler {
        
        /// <summary>
        /// Handles the __complete command (Cobra-style completion protocol).
        /// </summary>
        ResultVoid HandleComplete(ICliContext context, IEnumerable<string> args, object target);
        
        /// <summary>
        /// Handles the completion command with shell parameter.
        /// Supports: completion bash, completion bash install, completion bash uninstall
        /// </summary>
        ResultVoid HandleCompletion(ICliContext context, IEnumerable<string> args, object target);
    }
}
