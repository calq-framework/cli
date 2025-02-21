using System;
using CalqFramework.Cli;
using Xunit;

namespace CalqFramework.CliTest {

    public class CliOptionsDeserializerTest {

        [Fact]
        public void Deserialize_ArgumentsWithDoubleDashDelimiter_IgnoresOptionsAfterDelimiter() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(
                obj,
                $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}=10 -- --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc xyz".Split(' '),
                new OptionDeserializerConfiguration() { SkipUnknown = true }
            );
            Assert.Equal(10, obj.integer);
            Assert.Null(obj.text);
        }

        [Fact]
        public void Deserialize_UsingXUnitCommandLineArgsWithSkipUnknown_SetsPortToNonZero() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            OptionDeserializer.Deserialize(obj, new OptionDeserializerConfiguration() { SkipUnknown = true });
            Assert.NotEqual(0, obj.port);
        }

        [Fact]
        public void Deserialize_CombinedBooleanShortFlags_SetsAllBooleanPropertiesTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "-bx".Split(' '));
            Assert.True(obj.boolean);
            Assert.True(obj.xtrueBoolean);
        }

        [Fact]
        public void Deserialize_LongBooleanFlagWithoutValue_SetsBooleanPropertyTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.boolean))}".Split(' '));
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Deserialize_SingleBooleanShortFlag_SetsBooleanPropertyTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "-b".Split(' '));
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Deserialize_PlusPlusPrefixOnXTrueBoolean_SetFalse() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "++xtrue-boolean".Split(' '));
            Assert.False(obj.xtrueBoolean);
        }

        [Fact]
        public void Deserialize_PlusPrefixShortFlag_SetFalse() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "+x".Split(' '));
            Assert.False(obj.xtrueBoolean);
        }

        [Fact]
        public void Deserialize_MultipleBoolListValues_AppendsValuesToList() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.initializedBoolList))} false --{StringHelper.GetKebabCase(nameof(SomeConfiguration.initializedBoolList))} true".Split(' '));
            Assert.False(obj.initializedBoolList[2]);
            Assert.True(obj.initializedBoolList[3]);
        }

        [Fact]
        public void Deserialize_ValidArgumentsWithoutDelimiters_SetsAllPropertiesCorrectly() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}=10\t--{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc xyz".Split('\t')); // tab is used so that "abc xyz" is not split
            Assert.Equal(10, obj.integer);
            Assert.Equal("abc xyz", obj.text);
        }

        [Fact]
        public void Deserialize_CustomLongOptionName_SetsLongOptionPropertyTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "--customname".Split(' '));
            Assert.True(obj.longOption);
        }

        [Fact]
        public void Deserialize_CustomShortOptionName_SetsLongOptionPropertyTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "-c".Split(' '));
            Assert.True(obj.longOption);
        }

        [Fact]
        public void Deserialize_MultipleBooleanFlags_SetsAllPropertiesTrue() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "-b -x".Split(' '));
            Assert.True(obj.boolean);
            Assert.True(obj.xtrueBoolean);
        }

        [Fact]
        public void Deserialize_ShortOptionForShadowedField_SetsUsableOptionProperty() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, "-y".Split(' '));
            Assert.True(obj.shortOption);
        }

        [Fact]
        public void Deserialize_UnknownOptionWithSkipUnknown_ProcessesValidOptions() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(
                obj,
                $"--unknown --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc".Split(' '),
                new OptionDeserializerConfiguration() { SkipUnknown = true }
            );
            Assert.Equal("abc", obj.text);
        }

        [Fact]
        public void Deserialize_UnknownOptionWithValueAndSkipUnknown_ProcessesValidOptions() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(
                obj,
                $"--unknown value --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc".Split(' '),
                new OptionDeserializerConfiguration() { SkipUnknown = true }
            );
            Assert.Equal("abc", obj.text);
        }

        [Fact]
        public void Deserialize_UnknownOptionWithBooleanFlagAndSkipUnknown_SetsValidBooleanProperty() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(
                obj,
                $"--unknown -b --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc".Split(' '),
                new OptionDeserializerConfiguration() { SkipUnknown = true }
            );
            Assert.Equal("abc", obj.text);
            Assert.True(obj.boolean);
        }

        [Fact]
        public void Deserialize_UnknownOptionWithFlagInValueAndSkipUnknown_DoesNotSetBooleanProperty() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(
                obj,
                $"--unknown=-b --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc".Split(' '),
                new OptionDeserializerConfiguration() { SkipUnknown = true }
            );
            Assert.Equal("abc", obj.text);
            Assert.False(obj.boolean);
        }

        [Fact]
        public void Deserialize_ShadowedFieldOption_SetsUsableOptionNotOriginalField() {
            var obj = new SomeConfiguration();
            OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.shadowedfield))}".Split(' '));
            Assert.True(obj.usableOption);
            Assert.False(obj.shadowedfield);
        }

        [Fact]
        public void Deserialize_WithoutSkipUnknownAndInvalidArgs_ThrowsCliException() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            CliException ex = Assert.Throws<CliException>(() => {
                OptionDeserializer.Deserialize(obj);
            });
            Assert.NotEqual(0, obj.port);
        }

        [Fact]
        public void Deserialize_InvalidOptionInArguments_ThrowsCliException() {
            var obj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}=10 notanoption --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc xyz".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_StackedNonBooleanShortOptions_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, "-ib".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_LongOptionWithoutValue_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_UnexpectedPositionalValue_ThrowsCliException() {
            var obj = new SomeConfiguration();
            Assert.Throws<CliException>(() => {
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))} 10 notanoption --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc xyz".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_NonOptionProperty_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.inner))}=0".Split(' '));
            });
            Assert.Equal($"not an option: inner", ex.Message);
        }

        [Fact]
        public void Deserialize_InvalidDecimalFormatForInteger_ThrowsTypeMismatchException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}=0.1".Split(' '));
            });
            Assert.Equal($"value type mismatch: expected Int32 got 0.1", ex.Message);
        }

        [Fact]
        public void Deserialize_InvalidNonNumericValueForInteger_ThrowsTypeMismatchException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.integer))}=a".Split(' '));
            });
            Assert.Equal($"value type mismatch: expected Int32 got a", ex.Message);
        }

        [Fact]
        public void Deserialize_UnknownShortOption_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, "-m".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_ByteValueExceedsMaximum_ThrowsOutOfRangeException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, $"--{StringHelper.GetKebabCase(nameof(SomeConfiguration.aByteNumber))}=256".Split(' '));
            });
            Assert.Equal($"value is out of range: 256 (0-255)", ex.Message);
        }

        [Fact]
        public void Deserialize_PortValueExceedsMaximum_ThrowsOutOfRangeException() {
            var obj = new ConfigurationWithXUnitCommandLineArgs();
            CliException ex = Assert.Throws<CliException>(() => {
                OptionDeserializer.Deserialize(
                    obj,
                    $"--{StringHelper.GetKebabCase(nameof(ConfigurationWithXUnitCommandLineArgs.port))} {int.MaxValue}".Split(' '),
                    new OptionDeserializerConfiguration() { SkipUnknown = true }
                );
            });
            Assert.Equal($"value is out of range: 2147483647 (0-65535)", ex.Message);
        }

        [Fact]
        public void Deserialize_UnknownLongOptionWithValue_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, "--unknown=0".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_StackedOptionsWithNonBooleanInMiddle_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, "-bi".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_StackedOptionsStartingWithNonBoolean_ThrowsCliException() {
            CliException ex = Assert.Throws<CliException>(() => {
                var obj = new SomeConfiguration();
                OptionDeserializer.Deserialize(obj, "-ib 0".Split(' '));
            });
        }

        [Fact]
        public void Deserialize_UnexpectedValueWhenSkippingUnknown_ThrowsCliException() {
            var obj = new SomeConfiguration();
            CliException ex = Assert.Throws<CliException>(() => {
                OptionDeserializer.Deserialize(
                    obj,
                    $"unknown --{StringHelper.GetKebabCase(nameof(SomeConfiguration.text))}=abc".Split(' '),
                    new OptionDeserializerConfiguration() { SkipUnknown = true }
                );
            });
            Assert.Equal("unexpected value unknown", ex.Message);
        }
    }
}
