using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CalqFramework.Cli.Completion.Providers {

    /// <summary>
    /// Built-in completion provider that invokes an instance method.
    /// </summary>
    public class MethodCompletionProvider : ICompletionProvider {
        
        public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
            if (context.Filter == null) {
                return Enumerable.Empty<string>();
            }
            
            if (context.Submodule == null) {
                return Enumerable.Empty<string>();
            }
            
            var method = context.Submodule.GetType().GetMethod(
                context.Filter,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (method == null) {
                throw CliErrors.CompletionMethodNotFound(context.Filter, context.Submodule.GetType().Name);
            }
            
            var parameters = method.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) {
                throw CliErrors.InvalidCompletionMethodSignature(context.Filter, context.Submodule.GetType().Name);
            }
            
            var result = method.Invoke(context.Submodule, new object[] { context.PartialInput });
            
            if (result is not IEnumerable<string> completions) {
                throw CliErrors.InvalidCompletionMethodReturnType(context.Filter, context.Submodule.GetType().Name);
            }
            
            return completions;
        }
    }
}
