using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    /// <summary>
    /// Converts class member names to CLI-friendly string representations (e.g., kebab-case).
    /// </summary>
    public interface IClassMemberStringifier {
        /// <summary>
        /// Gets alternative names (aliases) for a field, including short forms.
        /// </summary>
        IEnumerable<string> GetAlternativeNames(FieldInfo info);
        /// <summary>
        /// Gets alternative names (aliases) for a method, including short forms.
        /// </summary>
        IEnumerable<string> GetAlternativeNames(MethodInfo info);
        /// <summary>
        /// Gets alternative names (aliases) for a parameter, including short forms.
        /// </summary>
        IEnumerable<string> GetAlternativeNames(ParameterInfo info);
        /// <summary>
        /// Gets alternative names (aliases) for a property, including short forms.
        /// </summary>
        IEnumerable<string> GetAlternativeNames(PropertyInfo info);
        /// <summary>
        /// Gets the required primary names for a field (from CliNameAttribute or kebab-case).
        /// </summary>
        IEnumerable<string> GetRequiredNames(FieldInfo info);
        /// <summary>
        /// Gets the required primary names for a method (from CliNameAttribute or kebab-case).
        /// </summary>
        IEnumerable<string> GetRequiredNames(MethodInfo info);
        /// <summary>
        /// Gets the required primary names for a property (from CliNameAttribute or kebab-case).
        /// </summary>
        IEnumerable<string> GetRequiredNames(PropertyInfo info);
        /// <summary>
        /// Gets the required primary names for a parameter (from CliNameAttribute or kebab-case).
        /// </summary>
        IEnumerable<string> GetRequiredNames(ParameterInfo info);
    }
}
