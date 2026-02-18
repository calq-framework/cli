using CalqFramework.Cli.DataAccess;

namespace CalqFramework.Cli {

    /// <summary>
    /// Provides context and configuration for CLI operations.
    /// </summary>
    public interface ICliContext {
        
        /// <summary>
        /// Factory for creating CLI component stores (options, subcommands, submodules).
        /// </summary>
        ICliComponentStoreFactory CliComponentStoreFactory { get; }
        
        /// <summary>
        /// Skip unknown options instead of throwing an exception.
        /// </summary>
        bool SkipUnknown { get; }
    }
}
