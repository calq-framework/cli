using System;
using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Prints completion suggestions based on CLI state and metadata.
    /// </summary>
    public class CompletionPrinter : ICompletionPrinter {

        public void PrintSubmodules(IEnumerable<Submodule> submodules, string partialInput) {
            var completions = submodules
                .SelectMany(s => s.Keys)
                .Where(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .OrderBy(k => k);

            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }

        public void PrintSubcommands(IEnumerable<Subcommand> subcommands, string partialInput) {
            var completions = subcommands
                .SelectMany(s => s.Keys)
                .Where(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .OrderBy(k => k);

            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }

        public void PrintOptions(IEnumerable<Option> options, string partialInput) {
            // Strip leading dashes from partial input for matching
            string partialInputStripped = partialInput.TrimStart('-', '+');
            
            var completions = options
                .SelectMany(o => o.Keys)
                .Where(k => k.StartsWith(partialInputStripped, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Length == 1 ? $"-{k}" : $"--{k}")  // Add appropriate prefix
                .Distinct()
                .OrderBy(k => k);

            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }

        public void PrintParameters(IEnumerable<Parameter> parameters, string partialInput) {
            // Strip leading dashes from partial input for matching
            string partialInputStripped = partialInput.TrimStart('-', '+');
            
            var completions = parameters
                .SelectMany(p => p.Keys)
                .Where(k => k.StartsWith(partialInputStripped, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Length == 1 ? $"-{k}" : $"--{k}")  // Add appropriate prefix
                .Distinct()
                .OrderBy(k => k);

            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }

        public void PrintSubmoduleValue(Submodule submodule, string partialInput) {
            // Submodules don't have values to complete
        }

        public void PrintSubcommandValue(Subcommand subcommand, string partialInput) {
            // Subcommands don't have values to complete
        }

        public void PrintOptionValue(Option option, string partialInput) {
            var completions = GetValueCompletions(option.Type, option.MemberInfo, partialInput);
            PrintCompletions(completions);
        }

        public void PrintParameterValue(Parameter parameter, string partialInput) {
            var completions = GetValueCompletions(parameter.Type, parameter.ParameterInfo, partialInput);
            PrintCompletions(completions);
        }

        private IEnumerable<string> GetValueCompletions(Type type, System.Reflection.ICustomAttributeProvider attributeProvider, string partialInput) {
            // Check for CliCompletionAttribute
            var completionAttr = attributeProvider
                .GetCustomAttributes(typeof(CliCompletionAttribute), false)
                .Cast<CliCompletionAttribute>()
                .FirstOrDefault();

            if (completionAttr != null) {
                var provider = (ICompletionProvider)Activator.CreateInstance(completionAttr.ProviderType)!;
                return provider.GetCompletions(partialInput);
            }

            // Auto-generate completions for enums
            if (type.IsEnum) {
                return Enum.GetNames(type)
                    .Where(name => name.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(name => name);
            }

            // Auto-generate completions for bool
            if (type == typeof(bool)) {
                return new[] { "true", "false" }
                    .Where(value => value.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
            }

            // No completions for other types
            return Enumerable.Empty<string>();
        }

        private void PrintCompletions(IEnumerable<string> completions) {
            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }
    }
}
