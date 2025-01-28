using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Cli.Serialization {
    internal interface ICliClassDataMemberSerializer {
        string GetCommandsString(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue, BindingFlags bindingAttr);
        string GetOptionsString(IEnumerable<MemberInfo> members, Func<MemberInfo, Type> getDataType, Func<MemberInfo, object?> getDataValue, BindingFlags bindingAttr);
    }
}