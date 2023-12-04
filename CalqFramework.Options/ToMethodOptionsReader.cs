using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options
{
    public class ToMethodOptionsReader : OptionsReaderBase {

        IEnumerable<ParameterInfo> parameters;

        public ToMethodOptionsReader(IEnumerable<ParameterInfo> parameters) : base() {
            this.parameters = parameters;
        }

        public static string GetOptionName(IEnumerable<ParameterInfo> parameters, string option)
        {
            if (option.Length == 1)
            {
                foreach (var param in parameters)
                {
                    if (param.Name![0] == option[0])
                    {
                        var result = param.Name;
                        return result;
                    }
                }
            }
            return option;
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
