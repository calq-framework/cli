using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Completion.Providers;
using CalqFramework.Cli.InterfaceComponents;

namespace CalqFramework.Cli.Completion;

/// <summary>
///     Prints completion suggestions based on CLI state and metadata.
/// </summary>
internal sealed class CompletionPrinter : ICompletionPrinter {
    public void PrintSubmodules(ICliContext context, IEnumerable<Submodule> submodules, string partialInput) {
        IOrderedEnumerable<string> completions = submodules
            .Select(s => s.Keys[0])
            .Where(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .OrderBy(k => k);

        foreach (string completion in completions) {
            context.Out.WriteLine(completion);
        }

        context.Out.Flush();
    }

    public void PrintSubcommands(ICliContext context, IEnumerable<Subcommand> subcommands, string partialInput) {
        IOrderedEnumerable<string> completions = subcommands
            .Select(s => s.Keys[0])
            .Where(k => k.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .OrderBy(k => k);

        foreach (string completion in completions) {
            context.Out.WriteLine(completion);
        }

        context.Out.Flush();
    }

    public void PrintParametersAndOptions(ICliContext context, IEnumerable<Parameter> parameters,
        IEnumerable<Option> options, string partialInput) {
        // Strip leading dashes from partial input for matching
        string partialInputStripped = partialInput.TrimStart('-', '+');

        // Combine parameters and options, deduplicate by key name (case-insensitive)
        IOrderedEnumerable<string> completions = parameters.Select(p => p.Keys[0])
            .Concat(options.Select(o => o.Keys[0]))
            .Where(k => k.StartsWith(partialInputStripped, StringComparison.OrdinalIgnoreCase))
            .Select(k => k.Length == 1 ? $"-{k}" : $"--{k}") // Add appropriate prefix
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(k => k);

        foreach (string completion in completions) {
            context.Out.WriteLine(completion);
        }

        context.Out.Flush();
    }

    public void PrintSubmoduleValue(ICliContext context, Submodule submodule, string partialInput) {
        // Submodules don't have values to complete
    }

    public void PrintSubcommandValue(ICliContext context, Subcommand subcommand, string partialInput) {
        // Subcommands don't have values to complete
    }

    public void PrintOptionValue(ICliContext context, Option option, string partialInput, object? submodule) {
        IEnumerable<string> completions =
            GetValueCompletions(option.ValueType, option.MemberInfo, partialInput, submodule);
        PrintCompletions(context, completions);
    }

    public void PrintParameterValue(ICliContext context, Parameter parameter, string partialInput, object? submodule) {
        IEnumerable<string> completions =
            GetValueCompletions(parameter.ValueType, parameter.ParameterInfo, partialInput, submodule);
        PrintCompletions(context, completions);
    }
    private static readonly string[] s_sourceArray = ["true", "false"];

    private static IEnumerable<string> GetValueCompletions(Type type, ICustomAttributeProvider attributeProvider,
        string partialInput, object? submodule) {
        CliCompletionAttribute? completionAttr = attributeProvider
            .GetCustomAttributes(typeof(CliCompletionAttribute), false)
            .Cast<CliCompletionAttribute>()
            .FirstOrDefault();

        if (completionAttr != null) {
            CompletionProviderContext context = new() {
                PartialInput = partialInput,
                Submodule = submodule,
                Filter = completionAttr.Filter
            };

            ICompletionProvider provider = (ICompletionProvider)Activator.CreateInstance(completionAttr.ProviderType)!;
            return provider.GetCompletions(context);
        }

        // Auto-detect FileInfo, DirectoryInfo, and FileSystemInfo types
        if (type == typeof(FileInfo)) {
            CompletionProviderContext context = new() {
                PartialInput = partialInput,
                Submodule = submodule,
                Filter = null
            };
            FileCompletionProvider provider = new();
            return provider.GetCompletions(context);
        }

        if (type == typeof(DirectoryInfo)) {
            CompletionProviderContext context = new() {
                PartialInput = partialInput,
                Submodule = submodule,
                Filter = null
            };
            DirectoryCompletionProvider provider = new();
            return provider.GetCompletions(context);
        }

        if (type == typeof(FileSystemInfo)) {
            CompletionProviderContext context = new() {
                PartialInput = partialInput,
                Submodule = submodule,
                Filter = null
            };
            FileSystemCompletionProvider provider = new();
            return provider.GetCompletions(context);
        }

        if (type.IsEnum) {
            return Enum.GetNames(type)
                .Where(name => name.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase))
                .OrderBy(name => name);
        }

        if (type == typeof(bool)) {
            return s_sourceArray.Where(value => value.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
        }

        return [];
    }

    private static void PrintCompletions(ICliContext context, IEnumerable<string> completions) {
        foreach (string completion in completions) {
            context.Out.WriteLine(completion);
        }

        context.Out.Flush();
    }
}
