using System;

namespace Ghbvft6.Calq.Options.Attributes {
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class NameAttribute : System.Attribute {
        public string Name { get; }

        public NameAttribute(string name) {
            Name = name;
        }
    }
}
