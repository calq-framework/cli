using CalqFramework.Options;
using CalqFramework.Serialization.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options {
    public class ToMethodOptionsReader : OptionsReaderBase {

        IEnumerable<ParameterInfo> parameters;

        public ToMethodOptionsReader(IEnumerable<ParameterInfo> parameters) : base() {
            this.parameters = parameters;
        }

        protected override bool TryGetOptionName(char option, out string result) {
            foreach (var param in parameters) {
                if (param.Name![0] == option) {
                    result = param.Name;
                    return true;
                }
            }
            result = default!;
            return false;
        }

        protected override bool TryGetOptionType(string option, out Type result) {
            foreach (var param in parameters) {
                if (param.Name == option) {
                    result = param.ParameterType;
                    return true;
                }
            }
            result = default!;
            return false;
        }
    }
}
