using System;
using System.Collections.Generic;
using System.Linq;
using CalqFramework.Cli.Completion.Providers;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Prints completion suggestions based on CLI state and metadata.
    /// </summary>
    public class CompletionPrinter : ICompletionPrinter {

        public void PrintSubmodules(IEnumerable<Submodule> submodules, string partialInput) {
            var completions = submodules
                .Select(s => s.Keys[0])
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
                .Select(s => s.Keys[0])
                .Where(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .OrderBy(k => k);

            foreach (var completion in completions) {
                Console.WriteLine(completion);
            }
            Console.Out.Flush();
        }

        public void PrintParametersAndOptions(IEnumerable<Parameter> parameters, IEnumerable<Option> options, string partialInput) {
            // Strip leading dashes from partial input for matching
            string partialInputStripped = partialInput.TrimStart('-', '+');
            
            // Combine parameters and options, deduplicate by key name (case-insensitive)
            var completions = parameters.Select(p => p.Keys[0])
                .Concat(options.Select(o => o.Keys[0]))
                .Where(k => k.StartsWith(partialInputStripped, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Length == 1 ? $"-{k}" : $"--{k}")  // Add appropriate prefix
                .Distinct(StringComparer.OrdinalIgnoreCase)
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

        public void PrintOptionValue(Option option, string partialInput, object? submodule) {
            var completions = GetValueCompletions(option.Type, option.MemberInfo, partialInput, submodule);
            PrintCompletions(completions);
        }

        public void PrintParameterValue(Parameter parameter, string partialInput, object? submodule) {
            var completions = GetValueCompletions(parameter.Type, parameter.ParameterInfo, partialInput, submodule);
            PrintCompletions(completions);
        }

        private IEnumerable<string> GetValueCompletions(Type type, System.Reflection.ICustomAttributeProvider attributeProvider, string partialInput, object? submodule) {
            var completionAttr = attributeProvider
                .GetCustomAttributes(typeof(CliCompletionAttribute), false)
                .Cast<CliCompletionAttribute>()
                .FirstOrDefault();

            if (completionAttr != null) {
                var context = new CompletionProviderContext {
                    PartialInput = partialInput,
                    Submodule = submodule,
                    Filter = completionAttr.Filter
                };
                
                var provider = (ICompletionProvider)Activator.CreateInstance(completionAttr.ProviderType)!;
                return provider.GetCompletions(context);
            }

            // Auto-detect FileInfo, DirectoryInfo, and FileSystemInfo types
            if (type == typeof(System.IO.FileInfo)) {
                var context = new CompletionProviderContext {
                    PartialInput = partialInput,
                    Submodule = submodule,
                    Filter = null
                };
                var provider = new FileCompletionProvider();
                return provider.GetCompletions(context);
            }

            if (type == typeof(System.IO.DirectoryInfo)) {
                var context = new CompletionProviderContext {
                    PartialInput = partialInput,
                    Submodule = submodule,
                    Filter = null
                };
                var provider = new DirectoryCompletionProvider();
                return provider.GetCompletions(context);
            }

            if (type == typeof(System.IO.FileSystemInfo)) {
                var context = new CompletionProviderContext {
                    PartialInput = partialInput,
                    Submodule = submodule,
                    Filter = null
                };
                var provider = new FileSystemCompletionProvider();
                return provider.GetCompletions(context);
            }

            if (type.IsEnum) {
                return Enum.GetNames(type)
                    .Where(name => name.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(name => name);
            }

            if (type == typeof(bool)) {
                return new[] { "true", "false" }
                    .Where(value => value.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
            }

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
