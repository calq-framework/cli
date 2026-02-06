using System;
using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli;
using CalqFramework.Cli.Completion.Providers;

namespace AutocompleteExample {

    /// <summary>Cloud provider enum for testing autocomplete.</summary>
    public enum CloudProvider {
        AWS,
        Azure,
        GCP,
        DigitalOcean,
        Linode
    }

    /// <summary>Verbosity level enum for testing autocomplete.</summary>
    public enum VerbosityLevel {
        Quiet,
        Normal,
        Verbose,
        Debug
    }

    /// <summary>Custom completion provider for region strings.</summary>
    public class RegionCompletionProvider : ICompletionProvider {
        public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
            var regions = new[] { 
                "us-east-1", "us-west-1", "us-west-2", 
                "eu-west-1", "eu-central-1", 
                "ap-southeast-1", "ap-northeast-1" 
            };
            return regions.Where(r => r.StartsWith(context.PartialInput, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>Custom completion provider for output format strings.</summary>
    public class FormatCompletionProvider : ICompletionProvider {
        public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
            var formats = new[] { 
                "json", "yaml", "xml", "table", "csv"
            };
            return formats.Where(f => f.StartsWith(context.PartialInput, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>CLI for cloud provider operations with autocomplete examples.</summary>
    public class CloudProviderCli {
        
        /// <summary>Verbosity level option (enum with autocomplete).</summary>
        public VerbosityLevel Verbosity { get; set; } = VerbosityLevel.Normal;

        /// <summary>Output format option (string with custom completion provider).</summary>
        [CliCompletion(typeof(FormatCompletionProvider))]
        public string Format { get; set; } = "json";

        /// <summary>Profile name option (string with method-based completion).</summary>
        [CliCompletion("GetProfileNames")]
        public string Profile { get; set; } = "default";

        /// <summary>Configuration file option (file path with extension filter).</summary>
        [CliCompletion(typeof(FileCompletionProvider), "*.json;*.yaml;*.yml")]
        public string? ConfigFile { get; set; }

        /// <summary>Instance method providing profile name completions.</summary>
        private IEnumerable<string> GetProfileNames(string partialInput) {
            var profiles = new[] { "default", "production", "staging", "development", "testing" };
            return profiles.Where(p => p.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Deploy resources to a cloud provider (enum parameter with autocomplete).</summary>
        /// <param name="provider">The cloud provider to deploy to.</param>
        /// <returns>Deployment result message.</returns>
        public string Deploy(CloudProvider provider) {
            var configInfo = ConfigFile != null ? $" (config: {ConfigFile})" : "";
            return $"[{Verbosity}] Deploying resources to {provider} using profile '{Profile}'...{configInfo} (format: {Format})";
        }

        /// <summary>Configure region for operations (string parameter with custom completion provider).</summary>
        /// <param name="region">The region to configure.</param>
        /// <returns>Configuration result message.</returns>
        public string ConfigureRegion([CliCompletion(typeof(RegionCompletionProvider))] string region) {
            return $"[{Verbosity}] Configured region: {region} for profile '{Profile}' (format: {Format})";
        }

        /// <summary>Backup configuration to a directory.</summary>
        /// <param name="outputDir">The directory to save the backup.</param>
        /// <returns>Backup result message.</returns>
        public string Backup([CliCompletion(typeof(DirectoryCompletionProvider))] string outputDir) {
            return $"[{Verbosity}] Backed up configuration to {outputDir} (format: {Format})";
        }

        /// <summary>Import configuration from a file or directory.</summary>
        /// <param name="path">The file or directory path to import from.</param>
        /// <returns>Import result message.</returns>
        public string Import([CliCompletion(typeof(FileSystemCompletionProvider), "*.json;*.yaml")] string path) {
            return $"[{Verbosity}] Imported configuration from {path} (format: {Format})";
        }

        /// <summary>Read a log file (auto-completes with FileInfo).</summary>
        /// <param name="logFile">The log file to read.</param>
        /// <returns>Log file read result.</returns>
        public string ReadLog(System.IO.FileInfo logFile) {
            return $"[{Verbosity}] Reading log file: {logFile.FullName} (format: {Format})";
        }

        /// <summary>List resources in a directory (auto-completes with DirectoryInfo).</summary>
        /// <param name="resourceDir">The directory containing resources.</param>
        /// <returns>Resource listing result.</returns>
        public string ListResources(System.IO.DirectoryInfo resourceDir) {
            return $"[{Verbosity}] Listing resources in: {resourceDir.FullName} (format: {Format})";
        }

        /// <summary>Process a file or directory (auto-completes with FileSystemInfo).</summary>
        /// <param name="target">The file or directory to process.</param>
        /// <returns>Processing result.</returns>
        public string Process(System.IO.FileSystemInfo target) {
            var type = target is System.IO.DirectoryInfo ? "directory" : "file";
            return $"[{Verbosity}] Processing {type}: {target.FullName} (format: {Format})";
        }
    }
}
