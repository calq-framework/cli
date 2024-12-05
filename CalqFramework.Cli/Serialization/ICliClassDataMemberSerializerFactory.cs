using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal interface ICliClassDataMemberSerializerFactory {
        ICliClassDataMemberSerializer CreateCliSerializer(Func<IEnumerable<MemberInfo>> getMembers, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue);
    }
}