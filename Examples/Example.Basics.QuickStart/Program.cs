using Example.Basics.QuickStart;

try {
    object? result = new CommandLineInterface().Execute(new QuickStartManager());
    if (result is not ValueTuple) {
        Console.WriteLine(JsonSerializer.Serialize(result));
    }
} catch (CliException ex) {
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
}

namespace Example.Basics.QuickStart {
    /// <summary>Displayed in the help menu.</summary>
    internal class QuickStartManager {
        /// <summary>Displayed in the help menu.</summary>
        public SubStart Submodule { get; } = new();

        public static void QuickRun() { }
    }

    internal class SubStart {
        public string DefaultValue { get; set; } = "default";

        [CliName("run")] // rename from 'sub-run' to 'run'
        [CliName("r")] // add alias 'r'
        public QuickResult SubRun(int requiredParameter, int optionalParameter = 1) => new(DefaultValue, requiredParameter, optionalParameter);
    }

    public record QuickResult(string s, int a, int b);
}
