using System;
using System.Collections.Generic;
using CalqFramework.Cli;
using CalqFramework.Cli.DataAccess.InterfaceComponent;
using Xunit;

namespace CalqFramework.CliTest {

    public class CommandLineInterfaceTest {

        [Fact]
        public void Execute_MethodWithCorrectCommandName_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}" });
        }

        [Fact]
        public void Execute_MethodWithLowercaseCommandName_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method).ToLower()}" });
        }

        [Fact]
        public void Execute_MethodWithOptionalParameter_OmitsOptionalParam() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithOptionalParam)}" });
        }

        [Fact]
        public void Execute_ObjectFieldWithNestedCommand_ExecutesNestedMethod() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.objectField)}", "NestedMethod" });
        }

        [Fact]
        public void Execute_HelpOption_GeneratesHelpOutput() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"--help" });
        }

        [Fact]
        public void Execute_MethodWithHelpOption_GeneratesMethodHelp() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}", $"--help" });
        }

        [Fact]
        public void Execute_MethodWithListParameter_AppendsMultipleValues() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new string[] { $"{nameof(SomeClassLibrary.MethodWithList)}", $"--paramList", "false", $"--paramList", "true" });
            Assert.False(((List<bool>)result)[0]);
            Assert.True(((List<bool>)result)[1]);
        }

        [Fact]
        public void Execute_MethodWithInitializedBoolList_UpdatesListElements() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new string[] { $"{nameof(SomeClassLibrary.Method)}", $"--{nameof(SomeClassLibrary.initializedBoolList)}", "false", $"--{nameof(SomeClassLibrary.initializedBoolList)}", "true" });
            Assert.False(tool.initializedBoolList[2]);
            Assert.True(tool.initializedBoolList[3]);
        }

        [Fact]
        public void Execute_MethodWithNamedParameters_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithIntegerAndText)}", $"--integer", "1", $"--text", "abc" });
            Assert.Null(result);
            Assert.Equal(1, tool.integerField);
            Assert.Equal("abc", tool.textField);
        }

        [Fact]
        public void Execute_MethodWithNamedInteger_ReturnsIntegerResult() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithInteger)}", $"--integer", "1" });
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_MethodWithPositionalParameter_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithInteger)}", "1" });
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_MethodWithPositionalTextAndBoolean_BindsBothParameters() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndBoolean)}", "abc", $"--boolean" });
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.True(tool.booleanField);
        }

        [Fact]
        public void Execute_MethodWithPositionalTextAndNamedInteger_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", "abc", $"--integer", "1" });
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.Equal(1, tool.integerField);
        }

        [Fact]
        public void Execute_MethodWithDelimiter_CapturesTextCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", "--", "--text", "-1" });
            Assert.Null(result);
            Assert.Equal("--text", tool.textField);
            Assert.Equal(-1, tool.integerField);
        }

        [Fact]
        public void Execute_MethodWithNamedText_ReturnsTextValue() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}", $"--text", "abc" });
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_MethodWithPositionalText_ReturnsTextValue() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}", "abc" });
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_MethodWithConflictingBooleanOption_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndBooleanError)}", "abc", $"--{nameof(SomeClassLibrary.booleanConflict)}" });
            });
        }

        [Fact]
        public void Execute_InvalidFieldCommand_ThrowsCliExceptionWithMessage() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.textField)}" });
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_LowercaseCommandWithDefaultBinding_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface() {
                    CliOptionsStoreFactory = new CliComponentStoreFactory { BindingFlags = CliComponentStoreFactory.DefaultLookup }
                }.Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method).ToLower()}" });
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_MethodWithMixedParameterOrder_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithIntegerAndText)}", "abc", $"--integer", "1" });
        }

        [Fact]
        public void Execute_MethodWithNamedIntegerBeforePositionalText_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, new[] {
                $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", $"--integer", "1", "abc"
            });
        }

        [Fact]
        public void Execute_MethodWithInvalidParameterOrder_ThrowsArgumentException() {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}", "abc" });
            });
        }

        [Fact]
        public void Execute_MethodWithMissingRequiredParameter_ThrowsArgumentException() {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}" });
            });
            Assert.Equal("unassigned parameter text", ex.Message);
        }

        [Fact]
        public void Execute_UnknownCommand_ThrowsCliExceptionWithInvalidCommandMessage() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"Unknown" });
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_NullObjectFieldWithNestedCommand_ThrowsNullReferenceException() {
            NullReferenceException ex = Assert.Throws<NullReferenceException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, new[] { $"{nameof(SomeClassLibrary.nullObjectField)}", "NestedMethod" });
            });
        }
    }
}
