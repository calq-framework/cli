namespace CalqFramework.Cli.Completion.Providers;

/// <summary>
///     Built-in completion provider that invokes an instance method.
/// </summary>
public sealed class MethodCompletionProvider : ICompletionProvider {
    public IEnumerable<string> GetCompletions(ICompletionProviderContext context) {
        if (context.Filter == null) {
            return [];
        }

        if (context.Submodule == null) {
            return [];
        }

        MethodInfo? method = context.Submodule.GetType()
            .GetMethod(context.Filter, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static) ?? throw CliErrors.CompletionMethodNotFound(
            context.Filter,
            context.Submodule.GetType()
                .Name);
        ParameterInfo[] parameters = method.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string)) {
            throw CliErrors.InvalidCompletionMethodSignature(
                context.Filter,
                context.Submodule.GetType()
                    .Name);
        }

        object? result = method.Invoke(context.Submodule, [context.PartialInput]);

        if (result is not IEnumerable<string> completions) {
            throw CliErrors.InvalidCompletionMethodReturnType(
                context.Filter,
                context.Submodule.GetType()
                    .Name);
        }

        return completions;
    }
}
