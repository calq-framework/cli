using CalqFramework.Options.DataMemberAccess;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CalqFramework.Options
{
    public class ToMethodOptionsReader : OptionsReaderBase {

        public DataMemberAndMethodParamsAccessor DataMemberAndMethodParamsAccessor { get; }

        public ToMethodOptionsReader(DataMemberAndMethodParamsAccessor dataMemberAndMethodParamsAccessor) : base() {
            DataMemberAndMethodParamsAccessor = dataMemberAndMethodParamsAccessor;
        }

        protected override bool ValidateOptionName(char option) {
            return DataMemberAndMethodParamsAccessor.TryResolveDataMemberKey(option.ToString(), out _);
        }

        protected override bool TryGetOptionType(string option, out Type result) {
            return DataMemberAndMethodParamsAccessor.TryGetDataType(option, out result);
        }
    }
}
