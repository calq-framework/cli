#pragma warning disable CS0649

using System.Collections.Generic;
using CalqFramework.Cli;
using CalqFramework.Cli.Completion.Providers;

namespace CalqFramework.CliTest {

    /// <summary>
    /// Some summary.
    /// </summary>
    internal class SomeClassLibrary {

        public class Nested {

            public static void NestedMethod() {
            }
        }

        public string textField;
        public int integerField;
        public bool booleanField;
        public bool booleanConflict;

        public Nested objectField = new();
        public Nested nullObjectField;

        public List<bool> initializedBoolList = new() { true, false };

        /// <summary>
        /// Does nothing.
        /// </summary>
        public static void Method() { }

        public static void MethodWithOptionalParam(bool optional = true) {
        }

        public static List<bool> MethodWithList(List<bool> paramList) {
            return paramList;
        }

        public static string MethodWithText(string text) {
            return text;
        }

        public static int MethodWithInteger(int integer) {
            return integer;
        }

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

        public static void MethodWithCustomCompletion([CliCompletion(typeof(EnvironmentCompletionProvider))] string environment) {
        }

        public static void MethodWithThreeParameters(string first, int second, bool third) {
        }

        // Instance method for providing completions
        private IEnumerable<string> GetRegionNames(string partialInput) {
            var regions = new[] { "us-east-1", "us-west-2", "eu-west-1", "ap-southeast-1" };
            return System.Linq.Enumerable.Where(regions, r => r.StartsWith(partialInput, System.StringComparison.OrdinalIgnoreCase));
        }

        // Command using method-based completion
        public void MethodWithMethodCompletion([CliCompletion("GetRegionNames")] string region) {
            textField = region;
        }

        // Command using CompletionProviders.Method with filter
        public void MethodWithCompletionProvidersMethod([CliCompletion(typeof(MethodCompletionProvider), "GetRegionNames")] string region) {
            textField = region;
        }

        // Commands using FileInfo, DirectoryInfo, and FileSystemInfo for auto-completion
        public static void MethodWithFileInfo(System.IO.FileInfo file) {
        }

        public static void MethodWithDirectoryInfo(System.IO.DirectoryInfo directory) {
        }

        public static void MethodWithFileSystemInfo(System.IO.FileSystemInfo path) {
        }

        // Commands using ICollection types for completion
        public static void MethodWithEnumList(List<LogLevel> levels) {
        }

        public static void MethodWithBoolList(List<bool> flags) {
        }
    }
}
