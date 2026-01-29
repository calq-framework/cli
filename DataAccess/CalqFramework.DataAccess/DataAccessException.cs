using System;

namespace CalqFramework.DataAccess {
    /// <summary>
    /// Exception thrown when data access operations fail.
    /// </summary>
    public class DataAccessException : Exception {
        public DataAccessException(string message) : base(message) { }
        
        public DataAccessException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}
