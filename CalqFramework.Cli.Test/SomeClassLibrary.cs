#pragma warning disable CS0649
#pragma warning disable IDE0060

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalqFramework.Cli.Completion.Providers;

namespace CalqFramework.Cli.Test;

/// <summary>
///     Some summary.
/// </summary>
internal sealed class SomeClassLibrary {
    public bool booleanConflict;
    public bool booleanField;

    public List<bool> initializedBoolList = [true, false];
    public int integerField;
    public Nested nullObjectField;

    public Nested objectField = new();

    public string textField;

    /// <summary>
    ///     Does nothing.
    /// </summary>
    public static void Method() {
    }

    public static void MethodWithOptionalParam(bool optional = true) {
    }

    public static List<bool> MethodWithList(List<bool> paramList) => paramList;

    public static IEnumerable<bool> MethodWithEnumerable(IEnumerable<bool> paramEnumerable) => paramEnumerable;

    public static string MethodWithText(string text) => text;

    public static int MethodWithInteger(int integer) => integer;

    public void MethodWithTextAndInteger(string text, int integer) {
        textField = text;
        integerField = integer;
    }

    public void MethodWithIntegerAndText(int integer, string text) {
        integerField = integer;
        textField = text;
    }

    public void MethodWithTextAndBoolean(string text, bool boolean = false) {
        textField = text;
        booleanField = boolean;
    }

    public void MethodWithTextAndBooleanError(string text, bool booleanConflict = false) {
        textField = text;
        this.booleanConflict = booleanConflict;
    }

    public static void MethodWithEnum(LogLevel level) {
    }

    public static void MethodWithCustomCompletion(
        [CliCompletion(typeof(EnvironmentCompletionProvider))] string environment) {
    }

    public static void MethodWithThreeParameters(string first, int second, bool third) {
    }

    // Instance method for providing completions
    private static IEnumerable<string> GetRegionNames(string partialInput) {
        string[] regions = ["us-east-1", "us-west-2", "eu-west-1", "ap-southeast-1"];
        return Enumerable.Where(regions, r => r.StartsWith(partialInput, StringComparison.OrdinalIgnoreCase));
    }

    // Command using method-based completion
    public void MethodWithMethodCompletion([CliCompletion("GetRegionNames")] string region) => textField = region;

    // Command using CompletionProviders.Method with filter
    public void MethodWithCompletionProvidersMethod(
        [CliCompletion(typeof(MethodCompletionProvider), "GetRegionNames")] string region) => textField = region;

    // Commands using FileInfo, DirectoryInfo, and FileSystemInfo for auto-completion
    public static void MethodWithFileInfo(FileInfo file) {
    }

    public static void MethodWithDirectoryInfo(DirectoryInfo directory) {
    }

    public static void MethodWithFileSystemInfo(FileSystemInfo path) {
    }

    // Commands using ICollection types for completion
    public static void MethodWithEnumList(List<LogLevel> levels) {
    }

    public static void MethodWithBoolList(List<bool> flags) {
    }

    public static ISet<string> MethodWithSet(ISet<string> tags) => tags;

    public class Nested {
        public static void NestedMethod() {
        }
    }
}
