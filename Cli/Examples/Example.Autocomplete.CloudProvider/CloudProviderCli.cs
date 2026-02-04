using System;
using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli.Completion;

namespace AutocompleteExample {

    /// <summary>Cloud provider enum for testing autocomplete.</summary>
    public enum CloudProvider {
        AWS,
        Azure,
        GCP,
        DigitalOcean,
        Linode
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

    /// <summary>CLI for cloud provider operations with autocomplete examples.</summary>
    public class CloudProviderCli {
        
        /// <summary>Cloud provider option (enum with autocomplete).</summary>
        public CloudProvider Provider { get; set; } = CloudProvider.AWS;

        /// <summary>Region option (string with custom completion provider).</summary>
        [CliCompletion(typeof(RegionCompletionProvider))]
        public string Region { get; set; } = "us-east-1";

        /// <summary>Deploy resources to a cloud provider (enum parameter with autocomplete).</summary>
        /// <param name="provider">The cloud provider to deploy to.</param>
        /// <returns>Deployment result message.</returns>
        public static string Deploy(CloudProvider provider) {
            return $"Deploying resources to {provider}...";
        }

        /// <summary>Configure region for operations (string parameter with custom completion provider).</summary>
        /// <param name="region">The region to configure.</param>
        /// <returns>Configuration result message.</returns>
        public static string ConfigureRegion([CliCompletion(typeof(RegionCompletionProvider))] string region) {
            return $"Configured region: {region}";
        }
    }
}
