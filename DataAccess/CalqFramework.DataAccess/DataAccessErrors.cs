using System;

namespace CalqFramework.DataAccess {

    /// <summary>
    /// Factory class for creating common DataAccessException instances with consistent messaging.
    /// </summary>
    public static class DataAccessErrors {

        /// <summary>
        /// Creates an exception for when a key is not found in a store.
        /// </summary>
        public static DataAccessException KeyNotFound<TKey>(TKey key) =>
            new DataAccessException($"Key '{key}' not found.");

        /// <summary>
        /// Creates an exception for ambiguous keys in dual key stores.
        /// </summary>
        public static DataAccessException AmbiguousKey(object? key) =>
            new DataAccessException($"Ambiguous key '{key}', enable shadowing to ignore this error");

        /// <summary>
        /// Creates an exception for unsupported collection types.
        /// </summary>
        public static ArgumentException UnsupportedCollectionType(string typeName) =>
            new ArgumentException($"Unsupported collection type: {typeName}", nameof(typeName));

        /// <summary>
        /// Creates an exception for when neither fields nor properties access is configured.
        /// </summary>
        public static ArgumentException NoAccessConfigured() =>
            new ArgumentException("Neither AccessFields nor AccessProperties is set");

        /// <summary>
        /// Creates an exception for invalid key types in list operations.
        /// </summary>
        public static ArgumentException InvalidListKey(string? keyTypeName) =>
            new ArgumentException($"Key must be an integer or parsable string for list removal, got {keyTypeName ?? "null"}");

        /// <summary>
        /// Creates an exception for unexpected arguments in method execution.
        /// </summary>
        public static ArgumentException UnexpectedArgument(object? argument) =>
            new ArgumentException($"Unexpected argument: {argument}");

        /// <summary>
        /// Creates an exception for unassigned required parameters.
        /// </summary>
        public static ArgumentException UnassignedParameter(string parameterName) =>
            new ArgumentException($"Unassigned parameter: {parameterName}");

        /// <summary>
        /// Creates an exception for unparsable types.
        /// </summary>
        public static ArgumentException TypeCannotBeParsed(string typeName) =>
            new ArgumentException($"Type cannot be parsed: {typeName}");

        /// <summary>
        /// Creates an exception for unrecognized member types.
        /// </summary>
        public static ArgumentException UnrecognizedMemberType() =>
            new ArgumentException("MemberInfo is not a recognized type");

        /// <summary>
        /// Creates an exception for unsupported operations on collections.
        /// </summary>
        public static NotSupportedException OperationNotSupported(string operation, string collectionTypeName) =>
            new NotSupportedException($"{operation} is not supported for {collectionTypeName}");
    }
}
