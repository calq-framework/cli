using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliDataMemberSerializerFactory : ICliDataMemberSerializerFactory {
        public BindingFlags BindingAttr { get; }

        public CliDataMemberSerializerFactory(BindingFlags bindingAttr) {
            BindingAttr = bindingAttr;
        }

        public ICliDataMemberSerializer CreateCliSerializer(Func<IEnumerable<MemberInfo>> getMembers, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue) {
            return new CliDataMemberSerializer(BindingAttr, getMembers, getDataType, getDataValue);
        }
    }
}
