#pragma warning disable CS0649

using CalqFramework.Options.Attributes;
using System.Collections.Generic;

namespace CalqFramework.OptionsTest {
    class TestConfiguration {
        public class Inner { }

        public Inner inner;
        public int integer;
        public byte aByteNumber;
        public bool boolean;
        public bool xtrueBoolean = true;
        public string text;
        [NameAttribute("customname")]
        public bool longOption;
        [NameAttribute("shadowedfield")]
        public bool usableOption;
        public bool shadowedfield;
        [NameAttribute("differentname")]
        [ShortNameAttribute('y')]
        public bool shortOption;
        public List<bool> initializedBoolList = new List<bool>() { true, false };
    }
}
