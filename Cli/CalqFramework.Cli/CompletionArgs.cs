namespace CalqFramework.Cli {

    /// <summary>
    /// Arguments for completion requests.
    /// </summary>
    public class CompletionArgs {
        
    
        public int Position { get; set; }
        

        public string Words { get; set; } = "";
    }

    /// <summary>
    /// Arguments for completion script generation.
    /// </summary>
    public class CompletionScriptArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }

    /// <summary>
    /// Arguments for completion installation.
    /// </summary>
    public class CompletionInstallArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }

    /// <summary>
    /// Arguments for completion uninstallation.
    /// </summary>
    public class CompletionUninstallArgs {
        public string Shell { get; set; } = "bash";
        public string? ProgramName { get; set; }
    }
}
