namespace CalqFramework.Cli {
    
    /// <summary>
    /// Arguments for completion requests.
    /// </summary>
    internal class CompletionArgs {
        public int Position { get; set; }
        public string Words { get; set; } = "";
    }

    /// <summary>
    /// Arguments for completion script generation.
    /// </summary>
    internal class CompletionScriptArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }

    /// <summary>
    /// Arguments for completion installation.
    /// </summary>
    internal class CompletionInstallArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }

    /// <summary>
    /// Arguments for completion uninstallation.
    /// </summary>
    internal class CompletionUninstallArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }
}
