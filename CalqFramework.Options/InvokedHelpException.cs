using System;
using System.Runtime.Serialization;

namespace CalqFramework.Options {
    [Serializable]
    internal class InvokedHelpException : Exception {
        public InvokedHelpException() {
        }

        public InvokedHelpException(string? message) : base(message) {
        }

        public InvokedHelpException(string? message, Exception? innerException) : base(message, innerException) {
        }

        protected InvokedHelpException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}