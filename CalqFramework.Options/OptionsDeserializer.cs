using CalqFramework.Options.DataMemberAccess;
using CalqFramework.Serialization.DataMemberAccess;
using CalqFramework.Serialization.Text;
using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;
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
                if (optionAttr.HasFlag(OptionFlags.NotAnOption)) {
                    throw new Exception("unexpected value");
                }

                Type type;
                try {
                    type = dataMemberAccessor.GetDataMemberType(typeof(T), option);
                } catch (MissingMemberException) {
                    if (options.SkipUnknown) {
                        continue;
                    }
                    throw;
                }

                var isCollection = type.GetInterface(nameof(ICollection)) != null;
                if (isCollection) {
                    type = type.GetGenericArguments()[0];
                }
                var valueObj = ValueParser.ParseValue(value, type, option);
                try {
                    if (isCollection == false) {
                        dataMemberAccessor.SetDataMemberValue(instance, option, valueObj);
                    } else {
                        var collection = dataMemberAccessor.GetDataMemberValue(instance, option);
                        CollectionMemberAccessor.AddChildValue((collection as ICollection)!, valueObj);
                    }
                } catch (ArgumentException ex) {
                    throw new Exception($"option and value type mismatch: {option}={value} ({option} is {type.Name})", ex);
                }
            }

            return reader.LastIndex;
        }
    }
}
