using CalqFramework.Cli;
using CalqFramework.Cli.Serialization;
using CalqFramework.Serialization.DataAccess.ClassMember;
using System;
using System.Collections.Generic;
using Xunit;

namespace CalqFramework.CliTest
{
    public class CommandLineInterfaceTest {
        [Fact]
        public void Execute_Should_SetTextProperty_When_TextOptionProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}", $"--text", "abc" });
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_Should_SetTextProperty_When_TextOptionProvidedWithoutLongName() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}", "abc" });
            Assert.Equal("abc", result);
        }

        [Fact]
        public void Execute_Should_SetIntegerProperty_When_IntegerOptionProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithInteger)}", $"--integer", "1" });
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_Should_SetIntegerProperty_When_IntegerOptionProvidedWithoutLongName() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithInteger)}", "1" });
            Assert.Equal(1, result);
        }

        [Fact]
        public void Execute_Should_SetTextAndIntegerProperties_When_TextAndIntegerOptionsProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", "abc", $"--integer", "1" });
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.Equal(1, tool.integerField);
        }

        [Fact]
        public void Execute_Should_SetIntegerAndTextProperties_When_IntegerAndTextOptionsProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithIntegerAndText)}", $"--integer", "1", $"--text", "abc" });
            Assert.Null(result);
            Assert.Equal(1, tool.integerField);
            Assert.Equal("abc", tool.textField);
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_InvalidUsage_NoIntegerOption() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithIntegerAndText)}", "abc", $"--integer", "1" });
            });
            // Assert.Equal("incorrect usage: expected Void IntegerAndText(Int32, System.String)", ex.Message);
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_InvalidUsage_NoTextOption() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithIntegerAndText)}", $"--integer", "1", "abc" });
            });
            // Assert.Equal("incorrect usage: expected Void IntegerAndText(Int32, System.String)", ex.Message);
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_InvalidUsage_NoIntegerValue() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", $"--integer", "1", "abc" });
        }

        [Fact]
        public void Execute_Should_CallMethod_When_MethodCommandProvided() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}" });
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_MethodCommandWithArgumentProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}", "abc" });
            });
            // Assert.Equal("incorrect usage: expected Void Method()", ex.Message);
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_UnknownCommandProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool, new[] { $"Unknown" });
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_Should_CallNestedMethodMethod_When_NestedMethodCommandProvided() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.objectField)}", "NestedMethod" });
        }

        [Fact]
        public void Execute_Should_ThrowNullReferenceException_When_NullNestedMethodCommandProvided() {
            var ex = Assert.Throws<NullReferenceException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.nullObjectField)}", "NestedMethod" });
            });
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_InvalidCommandCaseProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool,
                    new CliDeserializerOptions {
                        DataMemberStoreFactoryOptions = new DataMemberStoreFactoryOptions { BindingAttr = DataMemberStoreFactoryOptions.DefaultLookup }
                    },
                    new[] { $"{nameof(SomeClassLibrary.Method).ToLower()}" }
                );
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_Should_CallMethod_When_MethodCommandProvidedWithCustomOptions() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method).ToLower()}" });
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_UnassignedOptionProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithText)}" });
            });
            Assert.Equal("unassigned parameter text", ex.Message);
        }

        [Fact]
        public void Execute_Should_CallMethodWithOptionalParamMethod_When_MethodWithOptionalParamCommandProvided() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithOptionalParam)}" });
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_InternalTextOptionProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.textField)}" });
            });
            Assert.Equal("invalid command", ex.Message);
        }

        [Fact]
        public void Execute_Should_DisplayHelp_When_HelpOptionProvided() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"--help" });
        }

        [Fact]
        public void Execute_Should_DisplayHelpForMethod_When_HelpOptionProvidedForMethodCommand() {
            var tool = new SomeClassLibrary();
            CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.Method)}", $"--help" });
        }

        [Fact]
        public void Execute_Should_SetTextAndBooleanProperties_When_TextAndBooleanOptionsProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndBoolean)}", "abc", $"--boolean" });
            Assert.Null(result);
            Assert.Equal("abc", tool.textField);
            Assert.True(tool.booleanField);
        }

        [Fact]
        public void Execute_Should_ThrowCliException_When_BooleanOptionHasNamingConflict() {
            var ex = Assert.Throws<CliException>(() => {
                var tool = new SomeClassLibrary();
                var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndBooleanError)}", "abc", $"--{nameof(SomeClassLibrary.booleanConflict)}" });
            });
        }

        [Fact]
        public void Execute_Should_SetInitializedBoolList_When_MultipleValuesForListProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new string[] { $"{nameof(SomeClassLibrary.Method)}", $"--{nameof(SomeClassLibrary.initializedBoolList)}", "false", $"--{nameof(SomeClassLibrary.initializedBoolList)}", "true" });
            Assert.False(tool.initializedBoolList[2]);
            Assert.True(tool.initializedBoolList[3]);
        }

        [Fact]
        public void Execute_Should_ReturnList_When_MethodWithListCommandProvided() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new string[] { $"{nameof(SomeClassLibrary.MethodWithList)}", $"--paramList", "false", $"--paramList", "true" });
            Assert.False(((List<bool>)result)[0]);
            Assert.True(((List<bool>)result)[1]);
        }

        [Fact]
        public void Execute_Should_SetTextAndIntegerProperties_When_TextAndIntegerOptionsProvidedAfterDoubleDash() {
            var tool = new SomeClassLibrary();
            var result = CommandLineInterface.Execute(tool, new[] { $"{nameof(SomeClassLibrary.MethodWithTextAndInteger)}", "--", "--text", "-1" });
            Assert.Null(result);
            Assert.Equal("--text", tool.textField);
            Assert.Equal(-1, tool.integerField);
        }
    }
}
