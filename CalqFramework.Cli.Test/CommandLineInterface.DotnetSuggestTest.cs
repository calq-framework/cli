using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CalqFramework.Cli.Test;

[Collection("DotnetSuggest Tests")]
public class CommandLineInterfaceDotnetSuggestTest {
    private static List<string> GetDotnetSuggestCompletions(string directive, string commandLine) {
        SomeClassLibrary tool = new();

        using StringWriter writer = new();
        CommandLineInterface cli = new() { InterfaceOut = writer };
        cli.Execute(tool, new[] { directive, commandLine });

        string output = writer.ToString();
        return [.. output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)];
    }

    [Fact]
    public void HandleDotnetSuggest_BasicSuggestDirective_ReturnsSubcommands() {
        // Get actual executable name for test
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        List<string> completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} ");

        Assert.Contains("method", completions);
        Assert.Contains("method-with-text", completions);
    }

    [Fact]
    public void HandleDotnetSuggest_PartialSubcommand_ReturnsMatchingSubcommands() {
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        List<string> completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} meth");

        Assert.Contains("method", completions);
        Assert.Contains("method-with-text", completions);
        Assert.Contains("method-with-integer", completions);
    }

    [Fact]
    public void HandleDotnetSuggest_WithCursorPosition_CompletesUpToCursor() {
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        string commandLine = $"{exeName} method";
        // Cursor after "meth": position is length of "exeName meth"
        int cursorPos = $"{exeName} meth".Length;
        List<string> completions = GetDotnetSuggestCompletions($"[suggest:{cursorPos}]", commandLine);

        Assert.Contains("method", completions);
        Assert.Contains("method-with-text", completions);
    }

    [Fact]
    public void HandleDotnetSuggest_AfterSubcommand_ReturnsOptions() {
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        List<string> completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-text --");

        Assert.Contains("--text", completions);
    }

    [Fact]
    public void HandleDotnetSuggest_EnumParameter_ReturnsEnumValues() {
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        List<string> completions = GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-enum ");

        Assert.Contains("Debug", completions);
        Assert.Contains("Info", completions);
        Assert.Contains("Warning", completions);
        Assert.Contains("Error", completions);
    }

    [Fact]
    public void HandleDotnetSuggest_BoolParameter_ReturnsTrueFalse() {
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        List<string> completions =
            GetDotnetSuggestCompletions("[suggest]", $"{exeName} method-with-text-and-boolean abc --boolean ");

        Assert.Contains("true", completions);
        Assert.Contains("false", completions);
    }
    private static readonly string[] s_args = ["[suggest]"];

    [Fact]
    public void HandleDotnetSuggest_EmptyArgs_ReturnsVoid() {
        SomeClassLibrary tool = new();
        object result = new CommandLineInterface().Execute(tool, s_args);

        Assert.IsType<ResultVoid>(result);
    }

    [Fact]
    public void HandleDotnetSuggest_DirectiveWithoutClosingBracket_ReturnsVoid() {
        SomeClassLibrary tool = new();
        string exeName = Path.GetFileNameWithoutExtension(Environment.ProcessPath ?? "testhost");
        object result = new CommandLineInterface().Execute(tool, new[] { "[suggest", $"{exeName} " });

        Assert.IsType<ResultVoid>(result);
    }
}
