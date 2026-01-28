#pragma warning disable CS0649

using System.Collections.Generic;
using CalqFramework.Cli;

namespace CalqFramework.CliTest {

    internal class SomeConfiguration {

        public class Inner { }

        public Inner inner;
        public int integer;
        public byte aByteNumber;
        public bool boolean;
        public bool xtrueBoolean = true;
        public string text;

        [CliName("customname")]
        public bool longOption;

        [CliName("shadowedfield")]
        public bool usableOption;

        [CliName("s")]
        public bool shadowedfield;

        [CliName("differentname")]
        [CliName("y")]
        public bool shortOption;

        public List<bool> initializedBoolList = new() { true, false };
    }
}
