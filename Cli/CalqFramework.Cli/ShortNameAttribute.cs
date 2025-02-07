using System;

namespace CalqFramework.Cli {
    [AttributeUsage(AttributeTargets.All)]
    public class ShortNameAttribute : Attribute {
        public char Name { get; }

        public ShortNameAttribute(char name) {
            Name = name;
        }
    }
}
