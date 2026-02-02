using System;
using System.Collections.Generic;
using System.IO;
using CalqFramework.Cli;
using Xunit;

namespace CalqFramework.CliTest {

    public class CommandLineInterfaceCompletionTest {

        private string CaptureConsoleOutput(Action action) {
            var originalOut = Console.Out;
            using var writer = new StringWriter();
            Console.SetOut(writer);
            try {
                action();
                return writer.ToString();
            } finally {
                Console.SetOut(originalOut);
            }
        }

        private List<string> GetCompletions(string words, int position) {
            var tool = new SomeClassLibrary();
            var output = CaptureConsoleOutput(() => {
                var args = new List<string> { "completion", "complete", "--position", position.ToString(), "--words", words };
                new CommandLineInterface().Execute(tool, args);
            });
            return new List<string>(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        [Fact]
        public void ExecuteCompletion_RootLevel_ReturnsSubmodulesAndSubcommands() {
            var completions = GetCompletions("mycli ", 1);
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("object-field", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialSubcommand_ReturnsMatchingSubcommands() {
            var completions = GetCompletions("mycli meth", 1);
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("method-with-integer", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterSubmodule_ReturnsNestedSubcommands() {
            var completions = GetCompletions("mycli object-field ", 2);
            
            Assert.Contains("nested-method", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterSubcommand_ReturnsOptions() {
            var completions = GetCompletions("mycli method-with-text --", 2);
            
            Assert.Contains("--text", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialOption_ReturnsMatchingOptions() {
            var completions = GetCompletions("mycli method-with-integer --int", 2);
            
            Assert.Contains("--integer", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterOptionWithBoolParameter_ReturnsTrueFalse() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean ", 4);
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialBoolValue_ReturnsMatchingBoolValues() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean t", 4);
            
            Assert.Contains("true", completions);
            Assert.DoesNotContain("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_WithNamedParameter_CompletesNextPositionalParameter() {
            var completions = GetCompletions("mycli method-with-text-and-integer --integer 1 ", 4);
            
            // Should complete the text parameter (first positional), since integer was filled via option
            // No specific completions for string type, so list should be empty
            Assert.Empty(completions);
        }

        [Fact]
        public void ExecuteCompletion_InvalidSubcommand_ReturnsValidSubcommands() {
            var completions = GetCompletions("mycli invalid-command", 1);
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
        }

        [Fact]
        public void ExecuteCompletion_EnumParameter_ReturnsEnumValues() {
            var completions = GetCompletions("mycli method-with-enum ", 2);
            
            Assert.Contains("Debug", completions);
            Assert.Contains("Info", completions);
            Assert.Contains("Warning", completions);
            Assert.Contains("Error", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialEnumValue_ReturnsMatchingEnumValues() {
            var completions = GetCompletions("mycli method-with-enum Wa", 2);
            
            Assert.Contains("Warning", completions);
            Assert.DoesNotContain("Debug", completions);
            Assert.DoesNotContain("Info", completions);
            Assert.DoesNotContain("Error", completions);
        }

        [Fact]
        public void ExecuteCompletion_CustomCompletionProvider_ReturnsCustomValues() {
            var completions = GetCompletions("mycli method-with-custom-completion ", 2);
            
            Assert.Contains("development", completions);
            Assert.Contains("staging", completions);
            Assert.Contains("production", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialCustomCompletion_ReturnsMatchingValues() {
            var completions = GetCompletions("mycli method-with-custom-completion prod", 2);
            
            Assert.Contains("production", completions);
            Assert.DoesNotContain("development", completions);
            Assert.DoesNotContain("staging", completions);
        }

        [Fact]
        public void ExecuteCompletion_ShortOption_ReturnsShortAndLongOptions() {
            var completions = GetCompletions("mycli method-with-text -", 2);
            
            Assert.Contains("-t", completions);
            Assert.Contains("--text", completions);
        }

        [Fact]
        public void ExecuteCompletion_EmptyInputAtRoot_ReturnsAllSubcommands() {
            var completions = GetCompletions("mycli ", 1);
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("method-with-enum", completions);
            Assert.True(completions.Count > 5);
        }

        [Fact]
        public void ExecuteCompletion_ThirdParameter_CompletesThirdParameter() {
            var completions = GetCompletions("mycli method-with-three-parameters abc 123 ", 4);
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_NamedSecondParameter_CompletesFirstParameter() {
            var completions = GetCompletions("mycli method-with-three-parameters --second 123 ", 4);
            
            // Should complete the first parameter (string), which has no specific completions
            Assert.Empty(completions);
        }

        [Fact]
        public void ExecuteCompletion_FieldOptionValue_ReturnsFieldTypeCompletions() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean-field ", 4);
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }
    }
}
