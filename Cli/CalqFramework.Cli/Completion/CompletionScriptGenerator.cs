using System;
using System.IO;

namespace CalqFramework.Cli.Completion {

    /// <summary>
    /// Generates shell completion scripts for various shells.
    /// </summary>
    public class CompletionScriptGenerator : ICompletionScriptGenerator {

        public string GenerateBashScript(string programName) {
            return $@"# Bash completion script for {programName}
_{programName}_completion() {{
    local cur prev words cword
    _init_completion || return

    # Get the position of the current word
    local position=$((cword))
    
    # Join all words into a single string
    local all_words=""${{words[*]}}""
    
    # Call the completion command
    local completions=$({programName} completion complete --position ""$position"" --words ""$all_words"" 2>/dev/null)
    
    # Return the completions
    COMPREPLY=( $(compgen -W ""$completions"" -- ""$cur"") )
}}

complete -F _{programName}_completion {programName}
";
        }

        public string GenerateZshScript(string programName) {
            return $@"#compdef {programName}
# Zsh completion script for {programName}

_{programName}_completion() {{
    local -a completions
    local position=$((CURRENT))
    local words_str=""${{words[*]}}""
    
    # Call the completion command
    local output=$({programName} completion complete --position ""$position"" --words ""$words_str"" 2>/dev/null)
    
    # Convert output to array
    completions=(${{(f)output}})
    
    # Return completions
    _describe '{programName} commands' completions
}}

_{programName}_completion ""$@""
";
        }

        public string GeneratePowerShellScript(string programName) {
            return $@"# PowerShell completion script for {programName}
Register-ArgumentCompleter -Native -CommandName {programName} -ScriptBlock {{
    param($wordToComplete, $commandAst, $cursorPosition)
    
    $words = $commandAst.ToString() -split '\s+'
    $position = $words.Count - 1
    
    if ($wordToComplete) {{
        # If we're completing a partial word, position stays the same
    }} else {{
        # If we're at a space, increment position
        $position++
    }}
    
    $wordsStr = $commandAst.ToString()
    
    # Call the completion command
    $completions = & {programName} completion complete --position $position --words ""$wordsStr"" 2>$null
    
    $completions | ForEach-Object {{
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }}
}}
";
        }

        public string GenerateFishScript(string programName) {
            return $@"# Fish completion script for {programName}
function __{programName}_completion
    set -l tokens (commandline -opc)
    set -l position (count $tokens)
    set -l words_str (commandline -p)
    
    # Call the completion command
    {programName} completion complete --position $position --words ""$words_str"" 2>/dev/null
end

complete -c {programName} -f -a ""(__{programName}_completion)""
";
        }

        public string GetInstallPath(string shell, string programName) {
            return shell.ToLower() switch {
                "bash" => GetBashInstallPath(programName),
                "zsh" => GetZshInstallPath(programName),
                "powershell" or "pwsh" => GetPowerShellInstallPath(programName),
                "fish" => GetFishInstallPath(programName),
                _ => throw new ArgumentException($"Unsupported shell: {shell}")
            };
        }

        private string GetBashInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_completion.d", $"{programName}.bash");
            }
            return $"/etc/bash_completion.d/{programName}";
        }

        private string GetZshInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zsh", "completion", $"_{programName}");
            }
            return $"/usr/local/share/zsh/site-functions/_{programName}";
        }

        private string GetPowerShellInstallPath(string programName) {
            var profileDir = Path.GetDirectoryName(GetPowerShellProfilePath()) ?? "";
            return Path.Combine(profileDir, "Completions", $"{programName}.ps1");
        }

        private string GetFishInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fish", "completions", $"{programName}.fish");
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "fish", "completions", $"{programName}.fish");
        }

        private string GetPowerShellProfilePath() {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PowerShell", "Microsoft.PowerShell_profile.ps1");
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "powershell", "Microsoft.PowerShell_profile.ps1");
        }

        public void InstallScript(string shell, string programName, string script) {
            var installPath = GetInstallPath(shell, programName);
            var directory = Path.GetDirectoryName(installPath);
            
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(installPath, script);

            // For PowerShell, also add to profile if not already there
            if (shell.ToLower() is "powershell" or "pwsh") {
                AddToPowerShellProfile(installPath);
            }
        }

        private void AddToPowerShellProfile(string scriptPath) {
            var profilePath = GetPowerShellProfilePath();
            var sourceCommand = $". \"{scriptPath}\"";

            if (File.Exists(profilePath)) {
                var profileContent = File.ReadAllText(profilePath);
                if (!profileContent.Contains(scriptPath)) {
                    File.AppendAllText(profilePath, Environment.NewLine + sourceCommand + Environment.NewLine);
                }
            } else {
                var directory = Path.GetDirectoryName(profilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(profilePath, sourceCommand + Environment.NewLine);
            }
        }

        public bool UninstallScript(string shell, string programName) {
            var installPath = GetInstallPath(shell, programName);
            
            if (File.Exists(installPath)) {
                File.Delete(installPath);

                // For PowerShell, also remove from profile
                if (shell.ToLower() is "powershell" or "pwsh") {
                    RemoveFromPowerShellProfile(installPath);
                }

                return true;
            }

            return false;
        }

        private void RemoveFromPowerShellProfile(string scriptPath) {
            var profilePath = GetPowerShellProfilePath();
            
            if (File.Exists(profilePath)) {
                var profileContent = File.ReadAllText(profilePath);
                var sourceCommand = $". \"{scriptPath}\"";
                
                if (profileContent.Contains(sourceCommand)) {
                    profileContent = profileContent.Replace(sourceCommand + Environment.NewLine, "");
                    profileContent = profileContent.Replace(Environment.NewLine + sourceCommand, "");
                    profileContent = profileContent.Replace(sourceCommand, "");
                    File.WriteAllText(profilePath, profileContent);
                }
            }
        }
    }
}
