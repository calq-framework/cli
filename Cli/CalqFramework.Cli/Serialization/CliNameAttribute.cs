using System;

namespace CalqFramework.Cli.Serialization {

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CliNameAttribute : Attribute {

        public CliNameAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}
