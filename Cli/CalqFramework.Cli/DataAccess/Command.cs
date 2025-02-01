using System.Collections.Generic;
using System.Reflection;
using System;

namespace CalqFramework.Cli.DataAccess {
    public class Command {
        public IEnumerable<string> Keys { get; init; }
        public MemberInfo MemberInfo { get; init; }
    }
}