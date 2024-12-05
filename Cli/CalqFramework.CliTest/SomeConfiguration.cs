#pragma warning disable CS0649

using CalqFramework.Cli.Attributes;
using System.Collections.Generic;

namespace CalqFramework.CliTest {
    class SomeConfiguration {
        public class Inner { }

        public Inner inner;
        public int integer;
        public byte aByteNumber;
        public bool boolean;
        public bool xtrueBoolean = true;
        public string text;
        [Name("customname")]
        public bool longOption;
        [Name("shadowedfield")]
        public bool usableOption;
        public bool shadowedfield;
        [Name("differentname")]
        [ShortName('y')]
        public bool shortOption;
        public List<bool> initializedBoolList = new List<bool>() { true, false };
    }
}
