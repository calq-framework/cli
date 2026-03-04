using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CalqFramework.Cli.Completion.Providers;

/// <summary>
///     Built-in completion provider for both file and directory paths.
///     Filter applies only to files; all directories are included.
/// </summary>
public class FileSystemCompletionProvider : ICompletionProvider {
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
            return Enumerable.Empty<string>();
        }

        List<string> results = new();

        // Add all matching directories
        try {
            IEnumerable<string?> directories = Directory
                .EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(d => d != null && d.StartsWith(searchPrefix, StringComparison.OrdinalIgnoreCase));
            results.AddRange(directories!);
        } catch {
            // Ignore errors
        }

        // Add matching files (with filter applied)
        string[] patterns = string.IsNullOrEmpty(context.Filter)
            ? new[] { "*" }
            : context.Filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pattern in patterns) {
            try {
                IEnumerable<string?> matchingFiles = Directory
                    .EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(f => f != null && f.StartsWith(searchPrefix, StringComparison.OrdinalIgnoreCase));
                results.AddRange(matchingFiles!);
            } catch {
                // Ignore errors
            }
        }

        return results.Distinct().OrderBy(item => item);
    }
}
