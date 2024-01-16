using CalqFramework.Cli;
using CalqFramework.Cli.Serialization;
using System;
using Xunit;

namespace CalqFramework.CliTest
{
    public class CliOptionsDeserializerTest {
        [Fact]
        public void Should_SetBooleanProperty_When_LongOptionProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.boolean)}" });
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Should_SetBooleanProperty_When_ShortOptionProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-b" });
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Should_SetFalse_When_XTrueBooleanProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "+x" });
            Assert.False(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Should_SetBooleanProperties_When_StackedOptionsProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-bx" });
            Assert.True(targetObj.boolean);
            Assert.True(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Should_SetIntegerAndTextProperties_When_LongOptionsProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            Assert.Equal(10, targetObj.integer);
            Assert.Equal("abc xyz", targetObj.text);
        }

        [Fact]
        public void Should_SetIntegerProperty_When_LongOptionWithNoValueProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            Assert.Equal(10, targetObj.integer);
            Assert.Null(targetObj.text);
        }

        [Fact]
        public void Should_ThrowCliException_When_StackedOptionsAreNotAllBoolean() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-bi" });
            });
            //Assert.Equal("not all stacked options are boolean: bi", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionRequiresValue() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-ib" });
            });
            //Assert.Equal("option requires value: -ib", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_StackedOptionsAreNotAllBooleanWithProvidedValue() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-ib", "0" });
            });
            //Assert.Equal("not all stacked options are boolean: ib", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionRequiresValueButNotProvided() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}" });
            });
            //Assert.Equal("option requires value: --integer", ex.Message);
        }


        [Fact]
        public void Should_SetFalse_When_DoublePlusSignBeforeBooleanOption() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "++xtrueBoolean" });
            Assert.False(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingInnerPropertyWithStringValue() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.inner)}=0" });
            });
            Assert.Equal($"option and value type mismatch: inner=0 (inner is Inner)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingIntegerPropertyWithStringValue() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}=a" });
            });
            Assert.Equal($"option and value type mismatch: integer=a (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingIntegerPropertyWithDoubleValue() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}=0.1" });
            });
            Assert.Equal($"option and value type mismatch: integer=0.1 (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingOutOfRangeByteProperty() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.aByteNumber)}=256" });
            });
            Assert.Equal($"option value is out of range: aByteNumber=256 (0-255)", ex.Message);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingUnknownOption() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { "--unknown=0" });
            });
            // Assert.Equal($"option doesn't exist: unknown", ex.Message); // TODO check assert message
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingNonexistentShortOption() {
            var ex = Assert.Throws<CliException>(() => {
                var targetObj = new SomeConfiguration();
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-m" });
            });
            //Assert.Equal($"option doesn't exist: m", ex.Message);
        }

        [Fact]
        public void Should_SetMultipleBooleanProperties_When_MultipleOptionsProvided() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-b", "-x" });
            Assert.True(targetObj.boolean);
            Assert.True(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Should_ThrowCliException_When_InvalidOptionInTheMiddle() {
            var targetObj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}=10", "notanoption", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            });
        }

        [Fact]
        public void Should_ThrowCliException_When_OptionValueIsNotImmediatelyFollowing() {
            var targetObj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.integer)}", "10", "notanoption", $"--{nameof(SomeConfiguration.text)}=abc xyz" });
            });
        }

        [Fact]
        public void Should_ThrowCliException_When_DeserializeWithXUnitCommandLineArgs() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(targetObj);
            });
            // Assert.Contains("option doesn't exist", ex.Message); // TODO check assert message
            Assert.NotEqual(0, targetObj.port);
        }

        [Fact]
        public void Should_DeserializeWithXUnitCommandLineArgsAndSkipUnknownOptions() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            CliOptionsDeserializer.Deserialize(targetObj, new CliDeserializerOptions() {
                SkipUnknown = true
            });
            Assert.NotEqual(0, targetObj.port);
        }

        [Fact]
        public void Should_ThrowCliException_When_SettingOutOfRangePortValue() {
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<CliException>(() => {
                CliOptionsDeserializer.Deserialize(
                    targetObj, new CliDeserializerOptions() {
                        SkipUnknown = true
                    },
                    new string[] { $"--{nameof(ConfigurationWithXUnitCommandLineArgs.port)}", int.MaxValue.ToString() },
                    0
                );
            });
            Assert.Equal($"option value is out of range: port=2147483647 (0-65535)", ex.Message);
        }

        [Fact]
        public void Should_SetLongOptionToTrue_When_UsingCustomName() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--customname" });
            Assert.True(targetObj.longOption);
        }

        [Fact]
        public void Should_SetUsableOptionAndResetShadowedField_When_UsingShadowedFieldOption() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.shadowedfield)}" });
            Assert.True(targetObj.usableOption);
            Assert.False(targetObj.shadowedfield);
        }

        [Fact]
        public void Should_SetShortOptionToTrue_When_UsingShortName() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-y" });
            Assert.True(targetObj.shortOption);
        }

        [Fact]
        public void Should_SetLongOptionToTrue_When_UsingShortNameForLongOption() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { "-c" });
            Assert.True(targetObj.longOption);
        }

        [Fact]
        public void Should_DeserializeWithOptionsAndSkipUnknownOptions() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--{nameof(SomeConfiguration.integer)}=10", $"--", $"--{nameof(SomeConfiguration.text)}=abc xyz" },
                0
            );
            Assert.Equal(10, targetObj.integer);
            Assert.Null(targetObj.text);
        }

        [Fact]
        public void Should_SetTextOption_When_UsingUnknownOption() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", $"--{nameof(SomeConfiguration.text)}=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }

        [Fact]
        public void Should_SetTextOption_When_UsingUnknownOptionWithValue() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", "value", $"--{nameof(SomeConfiguration.text)}=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }
        [Fact]
        public void Should_SetTextOptionAndBoolean_When_UsingUnknownOptionAndKnownOption() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown", "-b", $"--{nameof(SomeConfiguration.text)}=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Should_SetTextOptionAndFalseBoolean_When_UsingUnknownOptionWithValue() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { $"--unknown=-b", $"--{nameof(SomeConfiguration.text)}=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
            Assert.False(targetObj.boolean);
        }

        [Fact]
        public void Should_SetTextOption_When_UsingUnknownOptionWithoutPrefix() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(
                targetObj, new CliDeserializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "unknown", $"--{nameof(SomeConfiguration.text)}=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }

        [Fact]
        public void Should_SetInitializedBoolList_When_UsingMultipleValuesForList() {
            var targetObj = new SomeConfiguration();
            CliOptionsDeserializer.Deserialize(targetObj, new string[] { $"--{nameof(SomeConfiguration.initializedBoolList)}", "false", $"--{nameof(SomeConfiguration.initializedBoolList)}", "true" });
            Assert.False(targetObj.initializedBoolList[2]);
            Assert.True(targetObj.initializedBoolList[3]);
        }
    }
}
