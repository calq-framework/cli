#pragma warning disable CS0649

namespace CalqFramework.Cli.Test;

internal sealed class SomeConfiguration {
    public byte aByteNumber;
    public bool boolean;

    public List<bool> initializedBoolList = [true, false];

    public Inner inner;
    public int integer;
    public LogLevel logLevel;

    [CliName("customname")] public bool longOption;

    public LogLevel? nullableLogLevel;

    [CliName("s")] public bool shadowedfield;

    [CliName("differentname")] [CliName("y")]
    public bool shortOption;

    public string text;

    [CliName("shadowedfield")] public bool usableOption;

    public bool xtrueBoolean = true;

    public class Inner {
    }
}
