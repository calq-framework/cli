using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;

namespace CalqFramework.Options {
    public class ToTypeOptionsReader : OptionsReaderBase {

        public DataMemberAccessorBase DataMemberAccessor { get; }

        public ToTypeOptionsReader(string[] args, DataMemberAccessorBase dataMemberAccessor) : base(args) {
            DataMemberAccessor = dataMemberAccessor;
        }

        //protected override bool TryGetOptionName(char option, out string result) {
        //    try {
        //        var member = DataMemberAccessor.GetDataMember(Type, option.ToString());
        //        result = member?.Name;
        //        return result != null ? true : false;
        //    } catch (Exception) {
        //        result = default!;
        //        return false;
        //    }
        //}

        protected override bool ValidateOptionName(char option)
        {
            return DataMemberAccessor.HasKey(option.ToString());
        }

        protected override bool HasOption(string option) {
            return DataMemberAccessor.HasKey(option);
        }

        protected override Type GetOptionType(string option) {
            return DataMemberAccessor.GetType(option);
        }
    }
}
