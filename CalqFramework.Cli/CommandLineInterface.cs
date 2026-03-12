using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CalqFramework.Cli.Completion;
using CalqFramework.Cli.DataAccess;
using CalqFramework.Cli.DataAccess.InterfaceComponentStores;
using CalqFramework.Cli.Formatting;
using CalqFramework.Cli.InterfaceComponents;
using CalqFramework.Cli.Parsing;
using CalqFramework.DataAccess;
using static CalqFramework.Cli.Parsing.OptionReaderBase;

namespace CalqFramework.Cli;

/// <summary>
///     Interprets CLI commands and executes methods on any classlib without requiring programming.
/// </summary>
public class CommandLineInterface : ICliContext {
    private readonly TextWriter? _out;

    private ICompletionHandler? _completionHandler;
    private IDotnetSuggestHandler? _dotnetSuggestHandler;
    private IHelpPrinter? _helpPrinter;

    public CommandLineInterface() => CliComponentStoreFactory = new CliComponentStoreFactory();

    /// <summary>
    ///     Help printer for displaying CLI help information.
    /// </summary>
    public IHelpPrinter HelpPrinter {
        get => _helpPrinter ??= new HelpPrinter();
        init => _helpPrinter = value;
    }

    public ICompletionHandler CompletionHandler {
        get => _completionHandler ??= new CompletionHandler();
        init => _completionHandler = value;
    }

    public IDotnetSuggestHandler DotnetSuggestHandler {
        get => _dotnetSuggestHandler ??= new DotnetSuggestHandler(CompletionHandler);
        init => _dotnetSuggestHandler = value;
    }

    /// <summary>
    ///     Include revision number in version output (4 digits instead of 3).
    /// </summary>
    public bool UseRevisionVersion { get; init; } = false;

    /// <summary>
    ///     Factory for creating CLI component stores (options, subcommands, submodules).
    /// </summary>
    public ICliComponentStoreFactory CliComponentStoreFactory { get; init; }

    /// <summary>
    ///     Skip unknown options instead of throwing an exception.
    /// </summary>
    public bool SkipUnknown { get; init; } = false;

    /// <summary>
    ///     TextWriter for interface description output (help, completions, framework messages).
    ///     Defaults to Console.Out if not specified.
    /// </summary>
    public TextWriter InterfaceOut {
        get => _out ?? Console.Out;
        init => _out = value;
    }

    /// <summary>
    ///     Executes a CLI command using command-line arguments from the environment.
    /// </summary>
    /// <returns>
    ///     The result of the executed command, or <see cref="ValueTuple"/> if the command returns void
    ///     or handles completion/help requests.
    /// </returns>
    public object? Execute(object target) => Execute(target, Environment.GetCommandLineArgs().Skip(1));

    /// <summary>
    ///     Executes a CLI command using the provided arguments.
    /// </summary>
    /// <returns>
    ///     The result of the executed command, or <see cref="ValueTuple"/> if the command returns void
    ///     or handles completion/help requests.
    /// </returns>
    public object? Execute(object target, IEnumerable<string> args) {
        List<string> argsList = [.. args];

        if (argsList.Count > 0) {
            string firstArg = argsList[0];

            switch (firstArg) {
                case "__complete":
                    CompletionHandler.HandleComplete(this, argsList, target);
                    break;

                case "completion":
                    CompletionHandler.HandleCompletion(this, argsList, target);
                    break;

                case string s when s.StartsWith("[suggest"):
                    DotnetSuggestHandler.HandleDotnetSuggest(this, argsList, target);
                    break;

                default:
                    return ExecuteInvoke(target, argsList);
            }
        }

        return default(ValueTuple);
    }

    /// <summary>
    ///     Executes a CLI command and invokes the subcommand.
    /// </summary>
    private object? ExecuteInvoke(object target, IEnumerable<string> args) {
        using IEnumerator<string> en = args.GetEnumerator();

        object? parentSubmodule = null;
        object submodule = target;
        ISubmoduleStore submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
        string? submoduleName = null;
        string? subcommandName = null;
        while (en.MoveNext()) {
            string arg = en.Current;
            if (submoduleStore.ContainsKey(arg)) {
                submoduleName = arg;
                parentSubmodule = submodule;
                submodule = submoduleStore[submoduleName]!;
                submoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(submodule);
            } else {
                subcommandName = arg;
                break;
            }
        }

        if (subcommandName == "--version" || subcommandName == "-v") {
            return Assembly.GetEntryAssembly()?.GetName().Version?.ToString(UseRevisionVersion ? 4 : 3);
        }

        ISubcommandStore subcommandStore = CliComponentStoreFactory.CreateSubcommandStore(submodule);

        if (subcommandName == null || subcommandName == "--help" || subcommandName == "-h") {
            IOptionStore optionStore = CliComponentStoreFactory.CreateOptionStore(submodule);
            bool isRoot = submodule == target;
            if (isRoot) {
                HelpPrinter.PrintHelp(this, target.GetType(), submoduleStore.GetSubmodules(),
                    subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor),
                    optionStore.GetOptions());
                return default(ValueTuple);
            }

            ISubmoduleStore parentSubmoduleStore = CliComponentStoreFactory.CreateSubmoduleStore(parentSubmodule!);
            Submodule submoduleInfo = parentSubmoduleStore.GetSubmodules()
                .Where(x => parentSubmoduleStore[x.Keys[0]] == submodule)
                .First(); // use the store to check for the key to comply with case sensitivity
            HelpPrinter.PrintHelp(this, target.GetType(), submoduleInfo, submoduleStore.GetSubmodules(),
                subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor),
                optionStore.GetOptions());
            return default(ValueTuple);
        }

        MethodInfo subcommand = subcommandStore[subcommandName!]!;
        ISubcommandExecutorWithOptions subcommandExecutorWithOptions =
            CliComponentStoreFactory.CreateSubcommandExecutorWithOptions(subcommand, submodule);

        bool hasArguments = en.MoveNext();
        if (hasArguments) {
            string firstArg = en.Current;

            if (firstArg == "--help" || firstArg == "-h") {
                HelpPrinter.PrintSubcommandHelp(this, target.GetType(),
                    subcommandStore.GetSubcommands(CliComponentStoreFactory.CreateSubcommandExecutor)
                        .Where(x => x.MethodInfo == subcommand).First(), subcommandExecutorWithOptions.GetOptions());
                return default(ValueTuple);
            }

            IEnumerator<string> skippedEn = GetSkippedEnumerator(en).GetEnumerator();
            ReadParametersAndOptions(skippedEn, subcommandExecutorWithOptions);
        }

        object? result;
        try {
            result = subcommandExecutorWithOptions.Invoke();
        } catch (DataAccessException ex) {
            throw CliErrors.FailedToAccessData(ex.Message, ex);
        }

        if (subcommand.ReturnType == typeof(void)) {
            return default(ValueTuple);
        }

        return result;
    }

    private static IEnumerable<string> GetSkippedEnumerator(IEnumerator<string> en) {
        do {
            yield return en.Current;
        } while (en.MoveNext());
    }

    private void ReadParametersAndOptions(IEnumerator<string> args,
        ISubcommandExecutorWithOptions subcommandExecutorWithOptions) {
        OptionReader optionReader = new(args, subcommandExecutorWithOptions);

        foreach ((string option, string value, OptionFlags optionAttr) in optionReader.Read()) {
            if (optionAttr.HasFlag(OptionFlags.AmbigousValue)) {
                throw CliErrors.AmbiguousValue(optionReader.ArgsEnumerator.Current, option);
            }

            if (optionAttr.HasFlag(OptionFlags.Unknown)) {
                if (SkipUnknown) {
                    continue;
                }

                throw CliErrors.UnknownOption(option);
            }

            if (optionAttr.HasFlag(OptionFlags.ValueUnassigned) && !optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                throw CliErrors.OptionRequiresValue(option);
            }

            if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                subcommandExecutorWithOptions.AddArgument(option);
            } else {
                try {
                    subcommandExecutorWithOptions[option] = value;
                } catch (DataAccessException ex) {
                    throw CliErrors.OptionValueError(option, value, ex.Message, ex);
                }
            }
        }

        while (args.MoveNext()) {
            subcommandExecutorWithOptions.AddArgument(args.Current);
        }
    }
}
