namespace CalqFramework.Cli.DataAccess.InterfaceComponentStores;

/// <summary>
///     Executes CLI subcommands with both parameter and option support (enables shadowing).
/// </summary>
public interface ISubcommandExecutorWithOptions : ISubcommandExecutor, IOptionStore {
}
