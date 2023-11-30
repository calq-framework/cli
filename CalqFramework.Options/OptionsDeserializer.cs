using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.Text;
using System;
using System.Collections;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static int Deserialize<T>(T instance) {
            return Deserialize(instance, new CliSerializerOptions());
        }

        public static int Deserialize<T>(T instance, CliSerializerOptions options) {
            return Deserialize(instance, options, Environment.GetCommandLineArgs(), 1);
        }

        public static int Deserialize<T>(T instance, string[] args, int startIndex = 0) {
            return Deserialize(instance, new CliSerializerOptions(), args, startIndex);
        }

        public static int Deserialize<T>(T instance, CliSerializerOptions options, string[] args, int startIndex = 0) {
            var dataMemberAccessor = CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);
            var reader = new ToTypeOptionsReader<T>(dataMemberAccessor);

            foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                if (optionAttr.HasFlag(OptionFlags.NotAnOption) || optionAttr.HasFlag(OptionFlags.Unknown)) {
                    if (options.SkipUnknown) {
                        continue;
                    } else {
                        throw new Exception("unexpected value");
                    }
                }

                if (optionAttr.HasFlag(OptionFlags.Unassigned)) {
                    throw new Exception("ambiguous syntax (try using --)");
                }

                var type = dataMemberAccessor.GetDataMemberType(typeof(T), option);

                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }

                var valueObj = ValueParser.ParseValue(value, type, option);

                if (isCollection == false) {
                    dataMemberAccessor.SetDataMemberValue(instance, option, valueObj);
                } else {
                    var collection = dataMemberAccessor.GetDataMemberValue(instance, option);
                    CollectionMemberAccessor.AddChildValue((collection as ICollection)!, valueObj);
                }
            }

            return reader.LastIndex;
        }
    }
}
