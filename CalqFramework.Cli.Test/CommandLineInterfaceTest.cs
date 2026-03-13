using CalqFramework.Cli.DataAccess;

namespace CalqFramework.Cli.Test;

public class CommandLineInterfaceTest {
    private static readonly bool[] s_expected = [false, true];

    [Fact]
    public void Execute_MethodWithCorrectCommandName_ExecutesSuccessfully() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))}".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithLowercaseCommandName_ExecutesSuccessfully() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method)).ToLower()}".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithOptionalParameter_OmitsOptionalParam() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithOptionalParam))}".Split(' '));
    }

    [Fact]
    public void Execute_ObjectFieldWithNestedCommand_ExecutesNestedMethod() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.objectField))} nested-method".Split(' '));
    }

    [Fact]
    public void Execute_HelpOption_GeneratesHelpOutput() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, "--help".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithHelpOption_GeneratesMethodHelp() {
        SomeClassLibrary tool = new();
        new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} --help".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithListParameter_AppendsMultipleValues() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithList))} --param-list false --param-list true".Split(' '));
        Assert.False(((List<bool>)result)[0]);
        Assert.True(((List<bool>)result)[1]);
    }

    [Fact]
    public void Execute_MethodWithEnumerableParameter_AcceptsMultipleValues() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithEnumerable))} --param-enumerable false --param-enumerable true".Split(' '));
        IEnumerable<bool> enumerable = (IEnumerable<bool>)result;
        Assert.Equal(s_expected, enumerable);
    }

    [Fact]
    public void Execute_MethodWithSetParameter_DeduplicatesValues() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithSet))} --tags urgent --tags important --tags urgent".Split(' '));
        ISet<string> set = (ISet<string>)result;
        Assert.Equal(2, set.Count);
        Assert.Contains("urgent", set);
        Assert.Contains("important", set);
    }


    [Fact]
    public void Execute_MethodWithInitializedBoolList_UpdatesListElements() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(
            tool,
            $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.initializedBoolList))} false --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.initializedBoolList))} true".Split(' '));
        Assert.False(tool.initializedBoolList[2]);
        Assert.True(tool.initializedBoolList[3]);
    }

    [Fact]
    public void Execute_MethodWithNamedParameters_BindsCorrectly() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithIntegerAndText))} --integer 1 --text abc".Split(' '));
        Assert.IsType<ValueTuple>(result);
        Assert.Equal(1, tool.integerField);
        Assert.Equal("abc", tool.textField);
    }

    [Fact]
    public void Execute_MethodWithNamedInteger_ReturnsIntegerResult() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithInteger))} --integer 1".Split(' '));
        Assert.Equal(1, result);
    }

    [Fact]
    public void Execute_MethodWithPositionalParameter_BindsCorrectly() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithInteger))} 1".Split(' '));
        Assert.Equal(1, result);
    }

    [Fact]
    public void Execute_MethodWithPositionalTextAndBoolean_BindsBothParameters() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndBoolean))} abc --boolean".Split(' '));
        Assert.IsType<ValueTuple>(result);
        Assert.Equal("abc", tool.textField);
        Assert.True(tool.booleanField);
    }

    [Fact]
    public void Execute_MethodWithPositionalTextAndNamedInteger_BindsCorrectly() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} abc --integer 1".Split(' '));
        Assert.IsType<ValueTuple>(result);
        Assert.Equal("abc", tool.textField);
        Assert.Equal(1, tool.integerField);
    }

    [Fact]
    public void Execute_MethodWithDelimiter_CapturesTextCorrectly() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} -- --text -1".Split(' '));
        Assert.IsType<ValueTuple>(result);
        Assert.Equal("--text", tool.textField);
        Assert.Equal(-1, tool.integerField);
    }

    [Fact]
    public void Execute_MethodWithNamedText_ReturnsTextValue() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))} --text abc".Split(' '));
        Assert.Equal("abc", result);
    }

    [Fact]
    public void Execute_MethodWithPositionalText_ReturnsTextValue() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))} abc".Split(' '));
        Assert.Equal("abc", result);
    }

    [Fact]
    public void Execute_MethodWithConflictingBooleanOption_ThrowsException() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndBooleanError))} abc --{StringHelper.GetKebabCase(nameof(SomeClassLibrary.booleanConflict))}".Split(' '));
        });
    }

    [Fact]
    public void Execute_InvalidFieldCommand_ThrowsCliExceptionWithMessage() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.textField))}".Split(' '));
        });
        Assert.Equal("Invalid subcommand: text-field", ex.Message);
    }

    [Fact]
    public void Execute_LowercaseCommandWithDefaultBinding_ThrowsCliException() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface {
                CliComponentStoreFactory = new CliComponentStoreFactory {
                    BindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public
                }
            }.Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method)).ToUpper()}".Split(' '));
        });
        Assert.Equal("Invalid subcommand: METHOD", ex.Message);
    }

    [Fact]
    public void Execute_MethodWithMixedParameterOrder_ExecutesSuccessfully() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithIntegerAndText))} abc --integer 1".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithNamedIntegerBeforePositionalText_ExecutesSuccessfully() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithTextAndInteger))} --integer 1 abc".Split(' '));
    }

    [Fact]
    public void Execute_MethodWithInvalidParameterOrder_ThrowsCliException() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} abc".Split(' '));
        });
    }

    [Fact]
    public void Execute_MethodWithMissingRequiredParameter_ThrowsCliException() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.MethodWithText))}".Split(' '));
        });
        Assert.Equal("Failed to access data: Unassigned parameter: text", ex.Message);
    }

    [Fact]
    public void Execute_UnknownCommand_ThrowsCliExceptionWithInvalidSubcommandMessage() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, "Unknown".Split(' '));
        });
        Assert.Equal("Invalid subcommand: Unknown", ex.Message);
    }

    [Fact]
    public void Execute_NullObjectFieldWithNestedCommand_ThrowsNullReferenceException() {
        NullReferenceException ex = Assert.Throws<NullReferenceException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.nullObjectField))} mested-method".Split(' '));
        });
    }

    [Fact]
    public void Execute_UnknownOptionWithValue_ThrowsUnknownOptionException() {
        CliException ex = Assert.Throws<CliException>(() => {
            SomeClassLibrary tool = new();
            new CommandLineInterface().Execute(tool, $"{StringHelper.GetKebabCase(nameof(SomeClassLibrary.Method))} --doesntexist a".Split(' '));
        });
        Assert.Equal("Unknown option: doesntexist", ex.Message);
    }
}
