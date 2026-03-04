using System.Reflection;

namespace CalqFramework.Cli.DataAccess;

/// <summary>
///     Validates whether a class member should be accessible in the CLI.
/// </summary>
public interface IAccessValidator {
    /// <summary>
    ///     Determines whether the specified member (field, property, or method) is valid for CLI access.
    /// </summary>
    bool IsValid(MemberInfo accessor);
}
