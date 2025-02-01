using CalqFramework.Cli;
using System;
using Xunit;

namespace CalqFramework.CliTest {
    public class CliOptionsDeserializerTest {
        [Fact]
        public void Should_SetBooleanProperty_When_LongOptionProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.boolean)}" });
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Should_SetBooleanProperty_When_ShortOptionProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "-b" });
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Should_SetFalse_When_XTrueBooleanProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "+x" });
            Assert.False(obj.xtrueBoolean);
        }

        [Fact]
        public void Should_SetBooleanProperties_When_StackedOptionsProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "-bx" });
            Assert.True(obj.boolean);
            Assert.True(obj.xtrueBoolean);
        }

        [Fact]
        public void Should_SetIntegerAndTextProperties_When_LongOptionsProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            Assert.Equal(10, obj.integer);
            Assert.Equal("abc xyz", obj.text);
        }

        [Fact]
        public void Should_SetIntegerProperty_When_LongOptionWithNoValueProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            Assert.Equal(10, obj.integer);
            Assert.Null(obj.text);
        }

        [Fact]
        public void Should_ThrowCliException_When_StackedOptionsAreNotAllBoolean() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { "-bi" });
            });
            //Assert.Equal("not all stacked options are boolean: bi", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionRequiresValue() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { "-ib" });
            });
            //Assert.Equal("option requires value: -ib", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_StackedOptionsAreNotAllBooleanWithProvidedValue() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { "-ib", "0" });
            });
            //Assert.Equal("not all stacked options are boolean: ib", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionRequiresValueButNotProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}" });
            });
            //Assert.Equal("option requires value: --integer", ex.Message);
        }


        [Fact]
        public void Should_SetFalse_When_DoublePlusSignBeforeBooleanOption() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "++xtrueBoolean" });
            Assert.False(obj.xtrueBoolean);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingInnerPropertyWithStringValue() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.inner)}=0" });
            });
            Assert.Equal($"not an option: inner", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingIntegerPropertyWithStringValue() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}=a" });
            });
            Assert.Equal($"option and value type mismatch: integer=a (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingIntegerPropertyWithDoubleValue() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}=0.1" });
            });
            Assert.Equal($"option and value type mismatch: integer=0.1 (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingOutOfRangeByteProperty() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.aByteNumber)}=256" });
            });
            Assert.Equal($"option value is out of range: aByteNumber=256 (0-255)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingUnknownOption() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { "--unknown=0" });
            });
            // Assert.Equal($"option doesn't exist: unknown", ex.Message); // TODO check assert message
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingNonexistentShortOption() {
            var ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(obj, new string[] { "-m" });
            });
            //Assert.Equal($"option doesn't exist: m", ex.Message);
        }

        [Fact]
        public void Should_SetMultipleBooleanProperties_When_MultipleOptionsProvided() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "-b", "-x" });
            Assert.True(obj.boolean);
            Assert.True(obj.xtrueBoolean);
        }

        [Fact]
        public void Should_ThrowCliException_When_InvalidOptionInTheMiddle() {
            var obj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", "notanoption", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            });
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionValueIsNotImmediatelyFollowing() {
            var obj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.integer)}", "10", "notanoption", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            });
        }

        [Fact]
        public void Should_ThrowCliException_When_DeserializeWithXUnitCommandLineArgs() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(obj);
            });
            // Assert.Contains("option doesn't exist", ex.Message); // TODO check assert message
            Assert.NotEqual(0, obj.port);
        }

        [Fact]
        public void Should_DeserializeWithXUnitCommandLineArgsAndSkipUnknownOptions() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            CliOptionsDeserializer.Deserialize(obj, new CliDeserializerOptions() {
                SkipUnknown = true
            });
            Assert.NotEqual(0, obj.port);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingOutOfRangePortValue() {
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(
                    obj, new CliDeserializerOptions() {
                        SkipUnknown = true
                    },
                    new string[] { $"--{nameof(ConfigurationWithXUnitCommandLineArgs.port)}", int.MaxValue.ToString() }
                );
            });
            Assert.Equal($"option value is out of range: port=2147483647 (0-65535)", ex.Message);
        }

        [Fact]
        public void Should_SetLongOptionToTrue_When_UsingCustomName() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--customname" });
            Assert.True(obj.longOption);
        }

        [Fact]
        public void Should_SetUsableOptionAndResetShadowedField_When_UsingShadowedFieldOption() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.shadowedfield)}" });
            Assert.True(obj.usableOption);
            Assert.False(obj.shadowedfield);
        }

        [Fact]
        public void Should_SetShortOptionToTrue_When_UsingShortName() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "-y" });
            Assert.True(obj.shortOption);
        }

        [Fact]
        public void Should_SetLongOptionToTrue_When_UsingShortNameForLongOption() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { "-c" });
            Assert.True(obj.longOption);
        }

        [Fact]
        public void Should_DeserializeWithOptionsAndSkipUnknownOptions() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                obj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--", $"--{nameof(SomeConfiguration.text)}=abc xyz" }
            );
            Assert.Equal(10, obj.integer);
            Assert.Null(obj.text);
        }

        [Fact]
        public void Should_SetTextOption_When_UsingUnknownOption() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                obj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", $"--{nameof(SomeConfiguration.text)}=abc" }
            );
            Assert.Equal("abc", obj.text);
        }

        [Fact]
        public void Should_SetTextOption_When_UsingUnknownOptionWithValue() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                obj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", "value", $"--{nameof(SomeConfiguration.text)}=abc" }
            );
            Assert.Equal("abc", obj.text);
        }
        [Fact]
        public void Should_SetTextOptionAndBoolean_When_UsingUnknownOptionAndKnownOption() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                obj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", "-b", $"--{nameof(SomeConfiguration.text)}=abc" }
            );
            Assert.Equal("abc", obj.text);
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Should_SetTextOptionAndFalseBoolean_When_UsingUnknownOptionWithValue() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                obj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown=-b", $"--{nameof(SomeConfiguration.text)}=abc" }
            );
            Assert.Equal("abc", obj.text);
            Assert.False(obj.boolean);
        }

        [Fact]
        public void Should_ThrowCliException_When_UsingUnknownOptionWithoutPrefix() {
            var obj = new SomeConfiguration();
            var ex = Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(
                    obj, new CliDeserializerOptions() {
                        SkipUnknown = true
                    },
                    new string[] { "unknown", $"--{nameof(SomeConfiguration.text)}=abc" }
                );
            });
            Assert.Equal("unexpected value unknown", ex.Message);
        }

        [Fact]
        public void Should_SetInitializedBoolList_When_UsingMultipleValuesForList() {
            var obj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(obj, new string[] { $"--{nameof(SomeConfiguration.initializedBoolList)}", "false", $"--{nameof(SomeConfiguration.initializedBoolList)}", "true" });
            Assert.False(obj.initializedBoolList[2]);
            Assert.True(obj.initializedBoolList[3]);
        }
    }
}
