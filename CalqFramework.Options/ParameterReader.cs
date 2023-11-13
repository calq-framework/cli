using CalqFramework.Options;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options {
    public class ParameterReader : ReaderBase {

        IEnumerable<ParameterInfo> parameters;

        public ParameterReader(IEnumerable<ParameterInfo> parameters) : base() {
            this.parameters = parameters;
        }

        protected override string GetOptionName(char option) {
            foreach (var param in parameters) {
                if (param.Name[0] == option) { // TODO when null?
                    return param.Name;
                }
            }
            throw new Exception($"option doesn't exist: {option}");
        }

        protected override Type GetOptionType(string option) {
            foreach (var param in parameters) {
                if (param.Name == option) {
                    return param.ParameterType;
                }
            }
            throw new Exception($"option doesn't exist: {option}");
        }
    }
}
