#pragma warning disable CS0649

using CalqFramework.Cli.Serialization;
using System.Collections.Generic;

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

        public bool shadowedfield;

        [CliName("differentname")]
        [CliName("y")]
        public bool shortOption;

        public List<bool> initializedBoolList = new List<bool>() { true, false };
    }
}