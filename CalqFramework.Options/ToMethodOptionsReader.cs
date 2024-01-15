using CalqFramework.Options.DataMemberAccess;
using System;

namespace CalqFramework.Options {
    public class ToMethodOptionsReader : OptionsReaderBase {

        public DataMemberAndMethodParamsAccessor DataMemberAndMethodParamsAccessor { get; }

        public ToMethodOptionsReader(string[] args, DataMemberAndMethodParamsAccessor dataMemberAndMethodParamsAccessor) : base(args) {
            DataMemberAndMethodParamsAccessor = dataMemberAndMethodParamsAccessor;
        }

        protected override bool ValidateOptionName(char option) {
            return DataMemberAndMethodParamsAccessor.HasKey(option.ToString());
        }

        protected override bool HasOption(string option) {
            return DataMemberAndMethodParamsAccessor.HasKey(option.ToString());
        }

        protected override Type GetOptionType(string option) {
            return DataMemberAndMethodParamsAccessor.GetType(option);
        }
    }
}
