namespace CalqFramework.Cli.Completion.Providers;

/// <summary>
///     Built-in completion provider for file paths.
/// </summary>
public sealed class FileCompletionProvider : ICompletionProvider {
    public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
        string partialPath = context.PartialInput ?? string.Empty;

        // Determine directory and search pattern
        string directory;
        string searchPrefix;

        if (string.IsNullOrEmpty(partialPath)) {
            directory = Directory.GetCurrentDirectory();
            searchPrefix = string.Empty;
        } else {
            string fullPath = Path.GetFullPath(partialPath);
            if (Directory.Exists(fullPath)) {
                directory = fullPath;
                searchPrefix = string.Empty;
            } else {
                directory = Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory();
                searchPrefix = Path.GetFileName(partialPath);
            }
        }

        if (!Directory.Exists(directory)) {
            return [];
        }

        // Get glob patterns from filter (semicolon-separated)
        string[] patterns = string.IsNullOrEmpty(context.Filter) ? ["*"] : context.Filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        List<string> files = [];
        foreach (string pattern in patterns) {
            try {
                IEnumerable<string?> matchingFiles = Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(f => f != null && f.StartsWith(searchPrefix, StringComparison.OrdinalIgnoreCase));
                files.AddRange(matchingFiles!);
            } catch {
                // Ignore errors (permission denied, invalid pattern, etc.)
            }
        }

        return files.Distinct()
            .OrderBy(f => f);
    }
}
