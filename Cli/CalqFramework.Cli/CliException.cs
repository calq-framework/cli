﻿using System;
using System.Runtime.Serialization;

namespace CalqFramework.Cli {

    [Serializable]
    public class CliException : Exception {

        public CliException() {
        }

        public CliException(string? message) : base(message) {
        }

        public CliException(string? message, Exception? innerException) : base(message, innerException) {
        }

        protected CliException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
