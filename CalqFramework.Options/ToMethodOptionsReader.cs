using CalqFramework.Options.DataMemberAccess;
using System;

namespace CalqFramework.Options {
    public class ToMethodOptionsReader : OptionsReaderBase {

        public DataMemberAndMethodParamsAccessor DataMemberAndMethodParamsAccessor { get; }

        public ToMethodOptionsReader(string[] args, DataMemberAndMethodParamsAccessor dataMemberAndMethodParamsAccessor) : base(args) {
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
