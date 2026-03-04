using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CalqFramework.Cli.Completion.Providers;

/// <summary>
///     Built-in completion provider for directory paths.
/// </summary>
public class DirectoryCompletionProvider : ICompletionProvider {
    public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
        string partialPath = context.PartialInput ?? string.Empty;

        // Determine directory and search prefix
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

        try {
            IEnumerable<string> directories = Directory
                .EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(d => d != null && d.StartsWith(searchPrefix, StringComparison.OrdinalIgnoreCase))
                .Select(d => d!);

            // Apply glob filter if provided
            if (!string.IsNullOrEmpty(context.Filter)) {
                string[] patterns = context.Filter.Split(';', StringSplitOptions.RemoveEmptyEntries);
                directories = directories.Where(d => patterns.Any(p => MatchesGlob(d, p)));
            }

            return directories.OrderBy(d => d);
        } catch {
            // Ignore errors (permission denied, etc.)
            return [];
        }
    }

    private static bool MatchesGlob(string name, string pattern) {
        // Simple glob matching: * matches any characters
        if (pattern == "*") {
            return true;
        }

        if (pattern.StartsWith('*') && pattern.EndsWith('*')) {
            string middle = pattern.Trim('*');
            return name.Contains(middle, StringComparison.OrdinalIgnoreCase);
        }

        if (pattern.StartsWith('*')) {
            string suffix = pattern.TrimStart('*');
            return name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        if (pattern.EndsWith('*')) {
            string prefix = pattern.TrimEnd('*');
            return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        return name.Equals(pattern, StringComparison.OrdinalIgnoreCase);
    }
}
