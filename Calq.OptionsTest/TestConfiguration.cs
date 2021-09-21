#pragma warning disable CS0649

using Ghbvft6.Calq.Options.Attributes;

namespace Ghbvft6.Calq.OptionsTest {
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
    }
}
