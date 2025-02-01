using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace CalqFramework.Cli.DataAccess {
    internal class CliOptionsAndActionParametersStore : DualKeyValueStore<string,  object?> {
        // FIXME this should me private and later deleted
        public CliMethodParameterStore _actionParameters;
        public ICliOptionsStore<string, object?, MemberInfo> Options { get; }
        public ICliParamStore<string, object?, ParameterInfo> ActionParameters { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] ParameterIndexByParameter { get => ActionParameters.ParameterIndexByParameter; }

        public CliOptionsAndActionParametersStore(ICliOptionsStore<string, object?, MemberInfo> options, CliMethodParameterStore actionParameters, object parentObj) : base(options, actionParameters)
        {
            _actionParameters = actionParameters;
            Options = options;
            ActionParameters = new CliParamStore<string, object?, ParameterInfo>(actionParameters);
            ParentObj = parentObj;
        }

        public object? Invoke()
        {
            return _actionParameters.Invoke(ParentObj);
        }

        //public string GetHelpString() {
        //    var result = "";

        //    result += ActionParameters.GetHelpString();
        //    result += "\n";
        //    result += Options.GetOptionsString();

        //    return result;
        //}
    }
}