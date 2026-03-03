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
        /// Built-in providers: <see cref="MethodCompletionProvider"/>, <see cref="FileCompletionProvider"/>, 
        /// <see cref="DirectoryCompletionProvider"/>, <see cref="FileSystemCompletionProvider"/>.
        /// Custom providers must implement <see cref="ICompletionProvider"/>.
        /// </summary>
        /// <param name="providerType">The type of the completion provider.</param>
        /// <param name="filter">Optional filter value (e.g., method name for <see cref="MethodCompletionProvider"/>, glob pattern for file providers).</param>
        public CliCompletionAttribute(Type providerType, string? filter = null) {
            if (!typeof(ICompletionProvider).IsAssignableFrom(providerType)) {
                throw CliErrors.InvalidCompletionProvider(providerType.Name);
            }
            ProviderType = providerType;
            Filter = filter;
        }

        /// <summary>
        /// Specifies an instance method name to call for completions.
        /// Shorthand for using <see cref="MethodCompletionProvider"/> with a method name filter.
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
        /// Gets the filter value (e.g., method name for method-based completion, glob pattern for file completion).
        /// </summary>
        public string? Filter { get; }
    }
}
