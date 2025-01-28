using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliClassDataMemberSerializerFactory : ICliClassDataMemberSerializerFactory {
        public BindingFlags BindingAttr { get; }

        public CliClassDataMemberSerializerFactory(BindingFlags bindingAttr) {
            BindingAttr = bindingAttr;
        }

        public ICliClassDataMemberSerializer CreateCliSerializer(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue) {
            return new CliClassDataMemberSerializer(BindingAttr, members, getDataType, getDataValue);
        }
    }
}
