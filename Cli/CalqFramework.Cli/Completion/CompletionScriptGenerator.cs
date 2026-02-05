using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CalqFramework.Cli.Completion {

    public class CompletionScriptGenerator : ICompletionScriptGenerator {

        private record ShellConfig(string Template, Func<string, string> GetInstallPath);

        private static readonly Dictionary<string, ShellConfig> _shellConfigs = new() {
            ["bash"] = new(BashTemplate, GetBashInstallPath),
            ["zsh"] = new(ZshTemplate, GetZshInstallPath),
            ["powershell"] = new(PowerShellTemplate, GetPowerShellInstallPath),
            ["fish"] = new(FishTemplate, GetFishInstallPath)
        };

        public IReadOnlyCollection<string> SupportedShells => _shellConfigs.Keys;

        public string GenerateScript(string shell, string programName) {
            var normalized = shell.ToLowerInvariant();
            if (!_shellConfigs.TryGetValue(normalized, out var config)) {
                throw CliErrors.UnsupportedShell(shell);
            }
            return config.Template.Replace("__PROGRAM_NAME__", programName);
        }

        public string GetInstallPath(string shell, string programName) {
            var normalized = shell.ToLowerInvariant();
            if (!_shellConfigs.TryGetValue(normalized, out var config)) {
                throw CliErrors.UnsupportedShell(shell);
            }
            return config.GetInstallPath(programName);
        }

        private const string BashTemplate = @"# Bash completion script for __PROGRAM_NAME__
___PROGRAM_NAME___completion() {
    local cur prev words cword
    _init_completion || return
    local position=$((cword))
    local all_words=""${words[*]}""
    local completions=$(__PROGRAM_NAME__ completion complete --position ""$position"" --words ""$all_words"" 2>/dev/null)
    COMPREPLY=( $(compgen -W ""$completions"" -- ""$cur"") )
}
complete -F ___PROGRAM_NAME___completion __PROGRAM_NAME__";

        private const string ZshTemplate = @"#compdef __PROGRAM_NAME__
___PROGRAM_NAME___completion() {
    local -a completions
    local position=$((CURRENT))
    local words_str=""${words[*]}""
    local output=$(__PROGRAM_NAME__ completion complete --position ""$position"" --words ""$words_str"" 2>/dev/null)
    completions=(${(f)output})
    _describe '__PROGRAM_NAME__ commands' completions
}
___PROGRAM_NAME___completion ""$@""";

        private const string PowerShellTemplate = @"# PowerShell completion script for __PROGRAM_NAME__
Register-ArgumentCompleter -Native -CommandName __PROGRAM_NAME__ -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    $words = $commandAst.ToString() -split '\s+'
    $position = $words.Count - 1
    if ($wordToComplete) {
    } else {
        $position++
    }
    $wordsStr = $commandAst.ToString()
    $completions = & __PROGRAM_NAME__ completion complete --position $position --words ""$wordsStr"" 2>$null
    $completions | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}";

        private const string FishTemplate = @"# Fish completion script for __PROGRAM_NAME__
function ____PROGRAM_NAME___completion
    set -l tokens (commandline -opc)
    set -l position (count $tokens)
    set -l words_str (commandline -p)
    __PROGRAM_NAME__ completion complete --position $position --words ""$words_str"" 2>/dev/null
end
complete -c __PROGRAM_NAME__ -f -a ""(____PROGRAM_NAME___completion)""";

        private static string GetBashInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".bash_completion.d", $"{programName}.bash");
            }
            return $"/etc/bash_completion.d/{programName}";
        }

        private static string GetZshInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".zsh", "completion", $"_{programName}");
            }
            return $"/usr/local/share/zsh/site-functions/_{programName}";
        }

        private static string GetPowerShellInstallPath(string programName) {
            var profileDir = Path.GetDirectoryName(GetPowerShellProfilePath()) ?? "";
            return Path.Combine(profileDir, "Completions", $"{programName}.ps1");
        }

        private static string GetFishInstallPath(string programName) {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fish", "completions", $"{programName}.fish");
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "fish", "completions", $"{programName}.fish");
        }

        private static string GetPowerShellProfilePath() {
            if (OperatingSystem.IsWindows()) {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PowerShell", "Microsoft.PowerShell_profile.ps1");
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "powershell", "Microsoft.PowerShell_profile.ps1");
        }

        public void InstallScript(string shell, string programName) {
            var script = GenerateScript(shell, programName);
            var installPath = GetInstallPath(shell, programName);
            var directory = Path.GetDirectoryName(installPath);
            
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(installPath, script);

            if (shell.ToLowerInvariant() == "powershell") {
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

                if (shell.ToLowerInvariant() == "powershell") {
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
