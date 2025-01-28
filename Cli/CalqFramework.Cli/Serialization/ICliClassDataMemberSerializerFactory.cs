using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal interface ICliClassDataMemberSerializerFactory {
        ICliClassDataMemberSerializer CreateCliSerializer(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue);
    }
}