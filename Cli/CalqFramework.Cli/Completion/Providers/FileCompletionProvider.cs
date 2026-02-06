using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CalqFramework.Cli.Completion.Providers {

    /// <summary>
    /// Built-in completion provider for file paths.
    /// </summary>
    public class FileCompletionProvider : ICompletionProvider {
        
        public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
            var partialPath = context.PartialInput ?? string.Empty;
            
            // Determine directory and search pattern
            string directory;
            string searchPrefix;
            
            if (string.IsNullOrEmpty(partialPath)) {
                directory = Directory.GetCurrentDirectory();
                searchPrefix = string.Empty;
            } else {
                var fullPath = Path.GetFullPath(partialPath);
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

            // Get glob patterns from filter (semicolon-separated)
            var patterns = string.IsNullOrEmpty(context.Filter) 
                ? new[] { "*" } 
                : context.Filter.Split(';', StringSplitOptions.RemoveEmptyEntries);

            var files = new List<string>();
            foreach (var pattern in patterns) {
                try {
                    var matchingFiles = Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly)
                        .Select(Path.GetFileName)
                        .Where(f => f != null && f.StartsWith(searchPrefix, StringComparison.OrdinalIgnoreCase));
                    files.AddRange(matchingFiles!);
                } catch {
                    // Ignore errors (permission denied, invalid pattern, etc.)
                }
            }

            return files.Distinct().OrderBy(f => f);
        }
    }
}
