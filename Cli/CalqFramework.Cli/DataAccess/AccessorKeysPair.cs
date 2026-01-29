using System.Collections.Generic;

namespace CalqFramework.Cli.DataAccess {

    /// <summary>
    /// Represents a pairing of a reflection accessor with its associated CLI key names.
    /// </summary>
    /// <typeparam name="TAccessor">The type of accessor (ParameterInfo, MethodInfo, or MemberInfo)</typeparam>
    public record AccessorKeysPair<TAccessor>(
        /// <summary>
        /// The reflection accessor (parameter, method, field, or property).
        /// </summary>
        TAccessor Accessor,

        /// <summary>
        /// The CLI key names that map to this accessor.
        /// </summary>
        IReadOnlyList<string> Keys
    );
}
