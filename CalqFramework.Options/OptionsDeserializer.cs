using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static IList<string> Deserialize(object instance) {
            return Deserialize(instance, new CliSerializerOptions());
        }

        public static IList<string> Deserialize(object instance, CliSerializerOptions options) {
            return Deserialize(instance, options, Environment.GetCommandLineArgs(), 1);
        }

        public static IList<string> Deserialize(object instance, string[] args, int startIndex = 0) {
            return Deserialize(instance, new CliSerializerOptions(), args, startIndex);
        }

        public static IList<string> Deserialize(object instance, CliSerializerOptions options, string[] args, int startIndex = 0) {
            var skippedOptions = new List<string>();
            var dataMemberAccessor = CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);
            var reader = new ToTypeOptionsReader(dataMemberAccessor, instance.GetType());

            foreach (var (option, value, optionAttr) in reader.Read(args, startIndex)) {
                if (optionAttr.HasFlag(OptionFlags.ValueUnassigned)) {
                    if (optionAttr.HasFlag(OptionFlags.AmbigousValue))
                    {
                        throw new Exception("ambiguous syntax (try using --)");
                    }
                    if (options.SkipUnknown) {
                        skippedOptions.Add(option);
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

            return skippedOptions;
        }
    }
}
