using System;

namespace CalqFramework.Cli {
    [AttributeUsage(AttributeTargets.All)]
    public class NameAttribute : Attribute {
        public string Name { get; }

        public NameAttribute(string name) {
            Name = name;
        }
    }
}
