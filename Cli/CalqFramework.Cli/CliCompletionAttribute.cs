using System;
using CalqFramework.Cli.Completion.Providers;

namespace CalqFramework.Cli {

    /// <summary>
    /// Attribute to specify completion provider for parameters and options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CliCompletionAttribute : Attribute {

        /// <summary>
        /// Specifies a completion provider type with optional filter.
        /// Use built-in providers from <see cref="CompletionProviders"/> or provide a custom type implementing <see cref="ICompletionProvider"/>.
        /// </summary>
        /// <param name="providerType">The type of the completion provider.</param>
        /// <param name="filter">Optional filter value (e.g., method name for <see cref="CompletionProviders.Method"/>).</param>
        public CliCompletionAttribute(Type providerType, string? filter = null) {
            if (!typeof(ICompletionProvider).IsAssignableFrom(providerType)) {
                throw CliErrors.InvalidCompletionProvider(providerType.Name);
            }
            ProviderType = providerType;
            Filter = filter;
        }

        /// <summary>
        /// Specifies an instance method name to call for completions.
        /// Equivalent to using <see cref="CompletionProviders.Method"/> with a filter.
        /// </summary>
        /// <param name="methodName">The name of the instance method to invoke for completions.</param>
        public CliCompletionAttribute(string methodName) 
            : this(typeof(MethodCompletionProvider), methodName) {
        }

        /// <summary>
        /// Gets the type of the completion provider.
        /// </summary>
        public Type ProviderType { get; }
        
        /// <summary>
        /// Gets the filter value (e.g., method name for method-based completion).
        /// </summary>
        public string? Filter { get; }
    }
}
