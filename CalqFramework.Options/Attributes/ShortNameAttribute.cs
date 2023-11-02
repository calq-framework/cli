using System;

namespace CalqFramework.Options.Attributes {
    [System.AttributeUsage(System.AttributeTargets.All)]
    public class ShortNameAttribute : System.Attribute {
        public char Name { get; }

        public ShortNameAttribute(char name) {
            Name = name;
        }
    }
}
