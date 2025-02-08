using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class OptionAndParameterStore : DualKeyValueStore<string, object?> {
        // FIXME this should me private and later deleted
        public CliMethodParameterStore _parameters;
        public IOptionStore<string, object?, MemberInfo> Options { get; }
        public IParameterStore<string, object?, ParameterInfo> Parameters { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] ParameterIndexByParameter { get => Parameters.ParameterIndexByParameter; }

        public OptionAndParameterStore(IOptionStore<string, object?, MemberInfo> options, CliMethodParameterStore actionParameters, object parentObj) : base(options, actionParameters) {
            _parameters = actionParameters;
            Options = options;
            Parameters = new ParameterStore<string, object?, ParameterInfo>(actionParameters);
            ParentObj = parentObj;
        }

        public object? Invoke() {
            return _parameters.Invoke(ParentObj);
        }
    }
}