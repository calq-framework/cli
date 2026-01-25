using System;

namespace CalqFramework.Cli.Serialization {

    /// <summary>
    /// Attribute to specify custom CLI names for class members (supports multiple aliases).
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class CliNameAttribute : Attribute {

        public CliNameAttribute(string name) {
            Name = name;
        }

        public string Name { get; }
    }
}
