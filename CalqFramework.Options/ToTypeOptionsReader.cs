using CalqFramework.Options.Attributes;
using CalqFramework.Serialization.DataMemberAccess;
using System;

namespace CalqFramework.Options {
    public class ToTypeOptionsReader<T> : OptionsReaderBase {

        public IDataMemberAccessor DataMemberAccessor { get; }

        private ToTypeOptionsReader() { }

        public ToTypeOptionsReader(IDataMemberAccessor dataMemberAccessor) {
            DataMemberAccessor = dataMemberAccessor;
        }

        protected override bool TryGetOptionName(char option, out string result) {
            try {
                var type = typeof(T);
                var member = DataMemberAccessor.GetDataMember(type, option.ToString());
                result = member?.Name;
                return result != null ? true : false;
            } catch (Exception) {
                result = default!;
                return false;
            }
        }

        protected override bool TryGetOptionType(string option, out Type result) {
            try {
                var type = typeof(T);
                result = DataMemberAccessor.GetDataMemberType(type, option);
                return true;
            } catch (Exception) {
                result = default!;
                return false;
            }
        }
    }
}
