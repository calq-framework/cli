using System;

namespace Ghbvft6.Calq.Options.Attributes {
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class ShortNameAttribute : System.Attribute {
        public char Name { get; }

        public ShortNameAttribute(char name) {
            Name = name;
        }
    }
}
