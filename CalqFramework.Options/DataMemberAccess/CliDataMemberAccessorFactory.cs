
using CalqFramework.Serialization.DataAccess;
using CalqFramework.Serialization.DataAccess.DataMemberAccess;
using System;

namespace CalqFramework.Options.DataMemberAccess {
    sealed public class CliDataMemberAccessorFactory : DataMemberAccessorFactoryBase {

        public static CliDataMemberAccessorFactory Instance { get; }

        static CliDataMemberAccessorFactory() {
            Instance = new CliDataMemberAccessorFactory(new DataMemberAccessorOptions());
        }

        public CliDataMemberAccessorFactory(DataMemberAccessorOptions dataMemberAccessorOptions) : base(dataMemberAccessorOptions) {
        }

        public DataMemberAccessorBase CreateDataMemberAccessor_FIXME(object obj) {
            if (DataMemberAccessorOptions.AccessFields && DataMemberAccessorOptions.AccessProperties) {
                throw new NotImplementedException();
            } else if (DataMemberAccessorOptions.AccessFields) {
                return CreateFieldAccessor(obj);
            } else if (DataMemberAccessorOptions.AccessProperties) {
                return CreatePropertyAccessor(obj);
            } else {
                throw new ArgumentException("Neither AccessFields nor AccessProperties is set.");
            }
        }

        protected override FieldAccessorBase CreateFieldAccessor(object obj) {
            return new CliFieldAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }
        protected override PropertyAccessorBase CreatePropertyAccessor(object obj) {
            return new CliPropertyAccessor(obj, DataMemberAccessorOptions.BindingAttr);
        }
    }
}
