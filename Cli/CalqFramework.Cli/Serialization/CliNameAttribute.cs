using System;

namespace CalqFramework.Cli.Serialization {
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CliNameAttribute : Attribute {
        public string Name { get; }

        public CliNameAttribute(string name) {
            Name = name;
        }
    }
}
