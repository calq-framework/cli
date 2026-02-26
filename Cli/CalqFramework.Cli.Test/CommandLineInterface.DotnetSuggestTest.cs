using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalqFramework.Cli;
using Xunit;

namespace CalqFramework.Cli.Test {

    [Collection("DotnetSuggest Tests")]
    public class CommandLineInterfaceDotnetSuggestTest {

        private List<string> GetDotnetSuggestCompletions(string directive, string commandLine) {
            var tool = new SomeClassLibrary();
            
            using var writer = new StringWriter();
            var cli = new CommandLineInterface { Out = writer };
            cli.Execute(tool, new[] { directive, commandLine });
            
            var output = writer.ToString();
            return new List<string>(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        [Fact]
        public void HandleDotnetSuggest_BasicSuggestDirective_ReturnsSubcommands() {
            // Get actual executable name for test
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} ");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_PartialSubcommand_ReturnsMatchingSubcommands() {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} meth");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("method-with-integer", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_WithCursorPosition_CompletesUpToCursor() {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var commandLine = $"{exeName} method";
            // Cursor after "meth": position is length of "exeName meth"
            var cursorPos = $"{exeName} meth".Length;
            var completions = GetDotnetSuggestCompletions($"[suggest:{cursorPos}]", commandLine);
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_AfterSubcommand_ReturnsOptions() {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-text --");
            
            Assert.Contains("--text", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_EnumParameter_ReturnsEnumValues() {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-enum ");
            
            Assert.Contains("Debug", completions);
            Assert.Contains("Info", completions);
            Assert.Contains("Warning", completions);
            Assert.Contains("Error", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_BoolParameter_ReturnsTrueFalse() {
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-text-and-boolean abc --boolean ");
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void HandleDotnetSuggest_EmptyArgs_ReturnsVoid() {
            var tool = new SomeClassLibrary();
            var result = new CommandLineInterface().Execute(tool, new[] { "[suggest]" });
            
            Assert.IsType<ResultVoid>(result);
        }

        [Fact]
        public void HandleDotnetSuggest_DirectiveWithoutClosingBracket_ReturnsVoid() {
            var tool = new SomeClassLibrary();
            var exeName = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
            var result = new CommandLineInterface().Execute(tool, new[] { "[suggest", $"{exeName} " });
            
            Assert.IsType<ResultVoid>(result);
        }
    }
}
