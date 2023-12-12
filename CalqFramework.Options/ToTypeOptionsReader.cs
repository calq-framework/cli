using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using System;

namespace CalqFramework.Options {
    public class ToTypeOptionsReader : OptionsReaderBase {

        public IDataMemberAccessor DataMemberAccessor { get; }
        public Type Type { get; }

        public ToTypeOptionsReader(string[] args, IDataMemberAccessor dataMemberAccessor, Type type) : base(args) {
            DataMemberAccessor = dataMemberAccessor;
            Type = type;
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
            var member = DataMemberAccessor.GetDataMember(Type, option.ToString());
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
                result = DataMemberAccessor.GetDataMemberType(Type, option);
                return true;
            } catch (Exception) {
                result = default!;
                return false;
            }
        }
    }
}
