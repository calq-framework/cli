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

        protected override string GetOptionName(char option) {
            var type = typeof(T);
            var member = DataMemberAccessor.GetDataMember(type, option.ToString());
            return member?.Name ?? throw new Exception($"option doesn't exist: {option}");
        }

        protected override Type GetOptionType(string option) {
            var type = typeof(T);
            return DataMemberAccessor.GetDataMemberType(type, option);
        }
    }
}
