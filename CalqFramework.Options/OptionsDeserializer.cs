using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static void Deserialize(object instance) {
            Deserialize(instance, new CliSerializerOptions());
        }

        public static void Deserialize(object instance, CliSerializerOptions options) {
            Deserialize(instance, options, Environment.GetCommandLineArgs(), 1);
        }

        public static void Deserialize(object instance, string[] args, int startIndex = 0) {
            Deserialize(instance, new CliSerializerOptions(), args, startIndex);
        }

        public static void Deserialize(object instance, CliSerializerOptions options, string[] args, int startIndex = 0) {
            var dataMemberAccessor = CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);
            var reader = new ToTypeOptionsReader(dataMemberAccessor, instance.GetType());

            foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                    if (optionAttr.HasFlag(OptionFlags.AmbigousValue))
                    {
                        throw new Exception("ambiguous syntax (try using --)");
                    }
                    if (options.SkipUnknown) {
                        continue;
                    } else {
                        throw new Exception("unexpected value");
                    }
                }

                var type = dataMemberAccessor.GetDataMemberType(instance.GetType(), option);

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
        }
    }
}
