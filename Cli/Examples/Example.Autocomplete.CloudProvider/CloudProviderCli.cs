using System;
using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli;

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
        public IEnumerable<string> GetCompletions(string partialInput) {
            var regions = new[] { 
                "us-east-1", "us-west-1", "us-west-2", 
                "eu-west-1", "eu-central-1", 
                "ap-southeast-1", "ap-northeast-1" 
            };
            return regions.Where(r => r.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>Custom completion provider for output format strings.</summary>
    public class FormatCompletionProvider : ICompletionProvider {
        public IEnumerable<string> GetCompletions(string partialInput) {
            var formats = new[] { 
                "json", "yaml", "xml", "table", "csv"
            };
            return formats.Where(f => f.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>CLI for cloud provider operations with autocomplete examples.</summary>
    public class CloudProviderCli {
        
        /// <summary>Verbosity level option (enum with autocomplete).</summary>
        public VerbosityLevel Verbosity { get; set; } = VerbosityLevel.Normal;

        /// <summary>Output format option (string with custom completion provider).</summary>
        [CliCompletion(typeof(FormatCompletionProvider))]
        public string Format { get; set; } = "json";

        /// <summary>Deploy resources to a cloud provider (enum parameter with autocomplete).</summary>
        /// <param name="provider">The cloud provider to deploy to.</param>
        /// <returns>Deployment result message.</returns>
        public string Deploy(CloudProvider provider) {
            return $"[{Verbosity}] Deploying resources to {provider}... (format: {Format})";
        }

        /// <summary>Configure region for operations (string parameter with custom completion provider).</summary>
        /// <param name="region">The region to configure.</param>
        /// <returns>Configuration result message.</returns>
        public string ConfigureRegion([CliCompletion(typeof(RegionCompletionProvider))] string region) {
            return $"[{Verbosity}] Configured region: {region} (format: {Format})";
        }
    }
}
