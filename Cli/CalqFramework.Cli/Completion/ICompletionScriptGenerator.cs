namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Interface for generating and managing shell completion scripts.
    /// </summary>
    public interface ICompletionScriptGenerator {
        
        /// <summary>
        /// Generates a Bash completion script.
        /// </summary>
        string GenerateBashScript(string programName);

        /// <summary>
        /// Generates a Zsh completion script.
        /// </summary>
        string GenerateZshScript(string programName);

        /// <summary>
        /// Generates a PowerShell completion script.
        /// </summary>
        string GeneratePowerShellScript(string programName);

        /// <summary>
        /// Generates a Fish completion script.
        /// </summary>
        string GenerateFishScript(string programName);

        /// <summary>
        /// Gets the installation path for a completion script.
        /// </summary>
        string GetInstallPath(string shell, string programName);

        /// <summary>
        /// Installs a completion script to the appropriate location.
        /// </summary>
        void InstallScript(string shell, string programName, string script);

        /// <summary>
        /// Uninstalls a completion script.
        /// </summary>
        bool UninstallScript(string shell, string programName);
    }
}
