using System.Collections.Generic;

namespace CalqFramework.Cli.Completion {

    public interface ICompletionScriptGenerator {
        
        IReadOnlyCollection<string> SupportedShells { get; }
        
        string GenerateScript(string shell, string programName);
        
        string GetInstallPath(string shell, string programName);
        
        void InstallScript(string shell, string programName);
        
        bool UninstallScript(string shell, string programName);
    }
}
