using System;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Attribute to specify completion provider for parameters and options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CliCompletionAttribute : Attribute {

        public CliCompletionAttribute(Type providerType) {
            if (!typeof(ICompletionProvider).IsAssignableFrom(providerType)) {
                throw CliErrors.InvalidCompletionProvider(providerType.Name);
            }
            ProviderType = providerType;
        }

        public Type ProviderType { get; }
    }
}
