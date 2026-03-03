using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli.Completion.Providers;

namespace CalqFramework.Cli.Test {
    
    public class EnvironmentCompletionProvider : ICompletionProvider {
        public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
            var environments = new[] { "development", "staging", "production" };
            return environments.Where(e => e.StartsWith(context.PartialInput, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
