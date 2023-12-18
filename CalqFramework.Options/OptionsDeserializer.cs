using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using static CalqFramework.Options.OptionsReaderBase;

namespace CalqFramework.Options {
    public class OptionsDeserializer {
        public static void Deserialize(object targetObj) {
            Deserialize(targetObj, new CliSerializerOptions());
        }

        public static void Deserialize(object targetObj, CliSerializerOptions options) {
            Deserialize(targetObj, options, Environment.GetCommandLineArgs(), 1);
        }

        public static void Deserialize(object targetObj, string[] args, int startIndex = 0) {
            Deserialize(targetObj, new CliSerializerOptions(), args, startIndex);
        }

        // TODO? pass DeserializationHandler and Reader - handler will use accessors
        // TODO? DataAccessor (not only DataMember)
        public static void Deserialize(object targetObj, CliSerializerOptions options, string[] args, int startIndex = 0) {
            var dataMemberAccessor = CliDataMemberAccessorFactory.Instance.CreateDataMemberAccessor(options);
            var reader = new ToTypeOptionsReader(args, dataMemberAccessor, targetObj.GetType()) { StartIndex = startIndex };

            foreach (var (option, value, optionAttr) in reader.Read()) {
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

                var type = dataMemberAccessor.GetDataMemberType(targetObj.GetType(), option);

                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }

                var valueObj = ValueParser.ParseValue(value, type, option);

                if (isCollection == false) {
                    dataMemberAccessor.SetDataMemberValue(targetObj, option, valueObj);
                } else {
                    var collection = dataMemberAccessor.GetDataMemberValue(targetObj, option);
                    CollectionMemberAccessor.AddChildValue((collection as ICollection)!, valueObj);
                }
            }
        }
    }
}
