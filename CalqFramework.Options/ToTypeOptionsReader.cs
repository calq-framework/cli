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
            var member = DataMemberAccessor.GetDataMember(option.ToString());
            if (member == null)
            {
                return false;
            } else
            {
                return true;
            }
        }

        protected override bool TryGetOptionType(string option, out Type result) {
            try {
                result = DataMemberAccessor.GetType(option);
                return true;
            } catch (Exception) {
                result = default!;
                return false;
            }
        }
    }
}
