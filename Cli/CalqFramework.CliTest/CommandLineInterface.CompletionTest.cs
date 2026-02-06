using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalqFramework.Cli;
using Xunit;

namespace CalqFramework.CliTest {

    [Collection("Completion Tests")]
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

        private List<string> GetCompletions(string commandLine) {
            var tool = new SomeClassLibrary();
            
            // Split preserving empty entries to handle trailing spaces
            var parts = commandLine.Split(' ');
            var args = parts.Skip(1).ToList();
            
            // If command line doesn't end with space and last part is not empty, that's what we're completing
            // If it ends with space, we're completing an empty string (already handled by split)
            
            var output = CaptureConsoleOutput(() => {
                new CommandLineInterface().Execute(tool, new[] { "__complete" }.Concat(args));
            });
            return new List<string>(output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        }

        [Fact]
        public void ExecuteCompletion_RootLevel_ReturnsSubmodulesAndSubcommands() {
            var completions = GetCompletions("mycli ");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("object-field", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialSubcommand_ReturnsMatchingSubcommands() {
            var completions = GetCompletions("mycli meth");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("method-with-integer", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterSubmodule_ReturnsNestedSubcommands() {
            var completions = GetCompletions("mycli object-field ");
            
            Assert.Contains("nested-method", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterSubcommand_ReturnsOptions() {
            var completions = GetCompletions("mycli method-with-text --");
            
            Assert.Contains("--text", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialOption_ReturnsMatchingOptions() {
            var completions = GetCompletions("mycli method-with-integer --int");
            
            Assert.Contains("--integer", completions);
        }

        [Fact]
        public void ExecuteCompletion_AfterOptionWithBoolParameter_ReturnsTrueFalse() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean ");
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialBoolValue_ReturnsMatchingBoolValues() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean t");
            
            Assert.Contains("true", completions);
            Assert.DoesNotContain("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_WithNamedParameter_CompletesNextPositionalParameter() {
            var completions = GetCompletions("mycli method-with-text-and-integer --integer 1 ");
            
            Assert.Empty(completions);
        }

        [Fact]
        public void ExecuteCompletion_InvalidSubcommand_ReturnsValidSubcommands() {
            var completions = GetCompletions("mycli invalid-command");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
        }

        [Fact]
        public void ExecuteCompletion_EnumParameter_ReturnsEnumValues() {
            var completions = GetCompletions("mycli method-with-enum ");
            
            Assert.Contains("Debug", completions);
            Assert.Contains("Info", completions);
            Assert.Contains("Warning", completions);
            Assert.Contains("Error", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialEnumValue_ReturnsMatchingEnumValues() {
            var completions = GetCompletions("mycli method-with-enum Wa");
            
            Assert.Contains("Warning", completions);
            Assert.DoesNotContain("Debug", completions);
            Assert.DoesNotContain("Info", completions);
            Assert.DoesNotContain("Error", completions);
        }

        [Fact]
        public void ExecuteCompletion_CustomCompletionProvider_ReturnsCustomValues() {
            var completions = GetCompletions("mycli method-with-custom-completion ");
            
            Assert.Contains("development", completions);
            Assert.Contains("staging", completions);
            Assert.Contains("production", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialCustomCompletion_ReturnsMatchingValues() {
            var completions = GetCompletions("mycli method-with-custom-completion prod");
            
            Assert.Contains("production", completions);
            Assert.DoesNotContain("development", completions);
            Assert.DoesNotContain("staging", completions);
        }

        [Fact]
        public void ExecuteCompletion_ShortOption_ReturnsShortAndLongOptions() {
            var completions = GetCompletions("mycli method-with-text -");
            
            Assert.Contains("--text", completions);
        }

        [Fact]
        public void ExecuteCompletion_EmptyInputAtRoot_ReturnsAllSubcommands() {
            var completions = GetCompletions("mycli ");
            
            Assert.Contains("method", completions);
            Assert.Contains("method-with-text", completions);
            Assert.Contains("method-with-enum", completions);
            Assert.True(completions.Count > 5);
        }

        [Fact]
        public void ExecuteCompletion_ThirdParameter_CompletesThirdParameter() {
            var completions = GetCompletions("mycli method-with-three-parameters abc 123 ");
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_NamedSecondParameter_CompletesFirstParameter() {
            var completions = GetCompletions("mycli method-with-three-parameters --second 123 ");
            
            Assert.Empty(completions);
        }

        [Fact]
        public void ExecuteCompletion_FieldOptionValue_ReturnsFieldTypeCompletions() {
            var completions = GetCompletions("mycli method-with-text-and-boolean abc --boolean-field ");
            
            Assert.Contains("true", completions);
            Assert.Contains("false", completions);
        }

        [Fact]
        public void ExecuteCompletion_MethodBasedCompletion_ReturnsMethodProvidedValues() {
            var completions = GetCompletions("mycli method-with-method-completion ");
            
            Assert.Contains("us-east-1", completions);
            Assert.Contains("us-west-2", completions);
            Assert.Contains("eu-west-1", completions);
            Assert.Contains("ap-southeast-1", completions);
        }

        [Fact]
        public void ExecuteCompletion_PartialMethodBasedCompletion_ReturnsMatchingValues() {
            var completions = GetCompletions("mycli method-with-method-completion eu");
            
            Assert.Contains("eu-west-1", completions);
            Assert.DoesNotContain("us-east-1", completions);
            Assert.DoesNotContain("us-west-2", completions);
            Assert.DoesNotContain("ap-southeast-1", completions);
        }

        [Fact]
        public void ExecuteCompletion_CompletionProvidersMethodSyntax_ReturnsMethodProvidedValues() {
            var completions = GetCompletions("mycli method-with-completion-providers-method ");
            
            Assert.Contains("us-east-1", completions);
            Assert.Contains("us-west-2", completions);
            Assert.Contains("eu-west-1", completions);
            Assert.Contains("ap-southeast-1", completions);
        }

        [Fact]
        public void ExecuteCompletion_CompletionProvidersMethodSyntaxPartial_ReturnsMatchingValues() {
            var completions = GetCompletions("mycli method-with-completion-providers-method ap");
            
            Assert.Contains("ap-southeast-1", completions);
            Assert.DoesNotContain("us-east-1", completions);
            Assert.DoesNotContain("us-west-2", completions);
            Assert.DoesNotContain("eu-west-1", completions);
        }
    }
}
