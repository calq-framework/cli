using CalqFramework.Cli.DataAccess.ClassMember;
using CalqFramework.DataAccess;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace CalqFramework.Cli.DataAccess.InterfaceComponent {
    internal class OptionAndParameterStore : DualKeyValueStore<string, string> {
        // FIXME this should me private and later deleted
        public ClassMember.ParameterStore _parameters;
        public IOptionStore Options { get; }
        public IParameterStore Parameters { get; }
        public object ParentObj { get; }

        //        public ParameterInfo[] ParameterIndexByParameter { get => Parameters.ParameterIndexByParameter; }

        public OptionAndParameterStore(IOptionStore options, ClassMember.ParameterStore actionParameters, object parentObj) : base(options, actionParameters) {
            _parameters = actionParameters;
            Options = options;
            Parameters = new ParameterStore(actionParameters);
            ParentObj = parentObj;
        }

        public object? Invoke() {
            return _parameters.Invoke(ParentObj);
        }
    }
}