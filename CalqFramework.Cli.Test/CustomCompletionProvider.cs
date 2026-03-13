using CalqFramework.Cli.Completion.Providers;

namespace CalqFramework.Cli.Test;

public class EnvironmentCompletionProvider : ICompletionProvider {
    public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
        string[] environments = ["development", "staging", "production"];
        return environments.Where(e => e.StartsWith(context.PartialInput, StringComparison.OrdinalIgnoreCase));
    }
}
