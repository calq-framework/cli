using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli;

namespace CalqFramework.CliTest {
    
    public class EnvironmentCompletionProvider : ICompletionProvider {
        public IEnumerable<string> GetCompletions(string partialInput) {
            var environments = new[] { "development", "staging", "production" };
            return environments.Where(e => e.StartsWith(partialInput, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
