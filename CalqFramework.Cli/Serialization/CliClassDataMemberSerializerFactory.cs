using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal class CliClassDataMemberSerializerFactory : ICliClassDataMemberSerializerFactory {
        public BindingFlags BindingAttr { get; }

        public CliClassDataMemberSerializerFactory(BindingFlags bindingAttr) {
            BindingAttr = bindingAttr;
        }

        public ICliClassDataMemberSerializer CreateCliSerializer(Func<IEnumerable<MemberInfo>> getMembers, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue) {
            return new CliClassDataMemberSerializer(BindingAttr, getMembers, getDataType, getDataValue);
        }
    }
}
