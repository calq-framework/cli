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
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))}".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithLowercaseCommandName_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method)).ToLower()}".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithOptionalParameter_OmitsOptionalParam() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithOptionalParam))}".Split(' '));
        }

        [Fact]
        public void Execute_ObjectFieldWithNestedCommand_ExecutesNestedMethod() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.objectField))} nested-method".Split(' '));
        }

        [Fact]
        public void Execute_HelpOption_GeneratesHelpOutput() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, "--help".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithHelpOption_GeneratesMethodHelp() {
            var tool = new SomeClassLibrary();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} --help".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithListParameter_AppendsMultipleValues() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithList))} --param-list false --param-list true".Split(' '));
            Assert.False(((List<bool>)result)[0]);
            Assert.True(((List<bool>)result)[1]);
        }

        [Fact]
        public void Execute_MethodWithInitializedBoolList_UpdatesListElements() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.initializedBoolList))} false --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.initializedBoolList))} true".Split(' '));
            Assert.False(tool.initializedBoolList[2]);
            Assert.True(tool.initializedBoolList[3]);
        }

        [Fact]
        public void Execute_MethodWithNamedParameters_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithIntegerAndText))} --integer 1 --text abc".Split(' '));
            Assert.Null(result);
            Assert.Equal(1, tool.integerField);
            Assert.Equal("abc", tool.textField);
        }

        [Fact]
        public void Execute_MethodWithNamedInteger_ReturnsIntegerResult() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithInteger))} --integer 1".Split(' '));
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_MethodWithPositionalParameter_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithInteger))} 1".Split(' '));
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_MethodWithPositionalTextAndBoolean_BindsBothParameters() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndBoolean))} abc --boolean".Split(' '));
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.True(tool.booleanField);
        }

        [Fact]
        public void Execute_MethodWithPositionalTextAndNamedInteger_BindsCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} abc --integer 1".Split(' '));
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.Equal(1, tool.integerField);
        }

        [Fact]
        public void Execute_MethodWithDelimiter_CapturesTextCorrectly() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} -- --text -1".Split(' '));
            Assert.Null(result);
            Assert.Equal("--text", tool.textField);
            Assert.Equal(-1, tool.integerField);
        }

        [Fact]
        public void Execute_MethodWithNamedText_ReturnsTextValue() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))} --text abc".Split(' '));
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_MethodWithPositionalText_ReturnsTextValue() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))} abc".Split(' '));
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_MethodWithConflictingBooleanOption_ThrowsException() {
            Exception ex = Assert.Throws<Exception>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndBooleanError))} abc --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.booleanConflict))}".Split(' '));
            });
        }

        [Fact]
        public void Execute_InvalidFieldCommand_ThrowsCliExceptionWithMessage() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.textField))}".Split(' '));
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_LowercaseCommandWithDefaultBinding_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface() {
                    CliComponentStoreFactory = new CliComponentStoreFactory { BindingFlags = CliComponentStoreFactory.DefaultLookup }
                }.Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method)).ToUpper()}".Split(' '));
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_MethodWithMixedParameterOrder_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithIntegerAndText))} abc --integer 1".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithNamedIntegerBeforePositionalText_ExecutesSuccessfully() {
            var tool = new SomeClassLibrary();
            object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} --integer 1 abc".Split(' '));
        }

        [Fact]
        public void Execute_MethodWithInvalidParameterOrder_ThrowsArgumentException() {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} abc".Split(' '));
            });
        }

        [Fact]
        public void Execute_MethodWithMissingRequiredParameter_ThrowsArgumentException() {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))}".Split(' '));
            });
            Assert.Equal("unassigned parameter text", ex.Message);
        }

        [Fact]
        public void Execute_UnknownCommand_ThrowsCliExceptionWithInvalidCommandMessage() {
            CliException ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, "Unknown".Split(' '));
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_NullObjectFieldWithNestedCommand_ThrowsNullReferenceException() {
            NullReferenceException ex = Assert.Throws<NullReferenceException>(() => {
                var tool = new SomeClassLibrary();
                new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.nullObjectField))} mested-method".Split(' '));
            });
        }
    }
}
