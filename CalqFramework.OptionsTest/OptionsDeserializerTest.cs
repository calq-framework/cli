using CalqFramework.Options;
using System;
using Xunit;

namespace CalqFramework.OptionsTest
{
    public class OptionsDeserializerTest {
        [Fact]
        public void Test1() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--boolean" });
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Test2() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "-b" });
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Test3() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "+x" });
            Assert.False(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Test4() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "-bx" });
            Assert.True(targetObj.boolean);
            Assert.True(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Test5() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer=10", "--text=abc xyz" });
            Assert.Equal(10, targetObj.integer);
            Assert.Equal("abc xyz", targetObj.text);
        }

        [Fact]
        public void Test6() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer=10", "--", "--text=abc xyz" });
            Assert.Equal(10, targetObj.integer);
            Assert.Null(targetObj.text);
        }

        [Fact]
        public void Test7() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "-bi" });
            });
            //Assert.Equal("not all stacked options are boolean: bi", ex.Message);
        }

        [Fact]
        public void Test8() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "-ib" });
            });
            //Assert.Equal("option requires value: -ib", ex.Message);
        }

        [Fact]
        public void Test9() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "-ib", "0" });
            });
            //Assert.Equal("not all stacked options are boolean: ib", ex.Message);
        }

        [Fact]
        public void Test10() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer" });
            });
            //Assert.Equal("option requires value: --integer", ex.Message);
        }

        [Fact]
        public void Test11() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "++xtrueBoolean" });
            Assert.False(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Test12() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--inner=0" });
            });
            Assert.Equal($"option and value type mismatch: inner=0 (inner is Inner)", ex.Message);
        }

        [Fact]
        public void Test13() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer=a" });
            });
            Assert.Equal($"option and value type mismatch: integer=a (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test14() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer=0.1" });
            });
            Assert.Equal($"option and value type mismatch: integer=0.1 (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test15() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--aByteNumber=256" });
            });
            Assert.Equal($"option value is out of range: aByteNumber=256 (0-255)", ex.Message);
        }

        [Fact]
        public void Test16() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--missingMemmber=0" });
            });
            // Assert.Equal($"option doesn't exist: missingMemmber", ex.Message); // TODO check assert message
        }

        [Fact]
        public void Test17() {
            var ex = Assert.Throws<Exception>(() => {
                var targetObj = new SomeConfiguration();
                OptionsDeserializer.Deserialize(targetObj, new string[] { "-m" });
            });
            //Assert.Equal($"option doesn't exist: m", ex.Message);
        }

        [Fact]
        public void Test18() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "-b", "-x" });
            Assert.True(targetObj.boolean);
            Assert.True(targetObj.xtrueBoolean);
        }

        [Fact]
        public void Test19() {
            var targetObj = new SomeConfiguration();
            Assert.Throws<Exception>(() => {
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer=10", "notanoption", "--text=abc xyz" });
            });
        }

        [Fact]
        public void Test20() {
            var targetObj = new SomeConfiguration();
            Assert.Throws<Exception>(() => {
                OptionsDeserializer.Deserialize(targetObj, new string[] { "--integer", "10", "notanoption", "--text=abc xyz" });
            });
        }

        [Fact]
        public void Test21() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<Exception>(() => {
                OptionsDeserializer.Deserialize(targetObj);
            });
            // Assert.Contains("option doesn't exist", ex.Message); // TODO check assert message
            Assert.NotEqual(0, targetObj.port);
        }

        [Fact]
        public void Test22() {
            Assert.NotEmpty(Environment.GetCommandLineArgs());
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            OptionsDeserializer.Deserialize(targetObj, new CliSerializerOptions() {
                SkipUnknown = true
            });
            Assert.NotEqual(0, targetObj.port);
        }

        [Fact]
        public void Test23() {
            var targetObj = new ConfigurationWithXUnitCommandLineArgs();
            var ex = Assert.Throws<Exception>(() => {
                OptionsDeserializer.Deserialize(
                    targetObj, new CliSerializerOptions() {
                        SkipUnknown = true
                    },
                    new string[] { "--port", int.MaxValue.ToString() },
                    0
                );
            });
            Assert.Equal($"option value is out of range: port=2147483647 (0-65535)", ex.Message);
        }

        [Fact]
        public void Test24() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--customname" });
            Assert.True(targetObj.longOption);
        }

        [Fact]
        public void Test25() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--shadowedfield" });
            Assert.True(targetObj.usableOption);
            Assert.False(targetObj.shadowedfield);
        }

        [Fact]
        public void Test26() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "-y" });
            Assert.True(targetObj.shortOption);
        }

        [Fact]
        public void Test27() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "-c" });
            Assert.True(targetObj.longOption);
        }

        [Fact]
        public void Test28() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "--integer=10", "--", "--text=abc xyz" },
                0
            );
            Assert.Equal(10, targetObj.integer);
            Assert.Null(targetObj.text);
        }

        [Fact]
        public void Test29() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "--unknown", "--text=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }

        [Fact]
        public void Test30() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "--unknown", "value", "--text=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }

        [Fact]
        public void Test31() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "--unknown", "-b", "--text=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
            Assert.True(targetObj.boolean);
        }

        [Fact]
        public void Test32() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "--unknown=-b", "--text=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
            Assert.False(targetObj.boolean);
        }

        [Fact]
        public void Test33() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(
                targetObj, new CliSerializerOptions() {
                    SkipUnknown = true
                },
                new string[] { "unknown", "--text=abc" },
                0
            );
            Assert.Equal("abc", targetObj.text);
        }

        [Fact]
        public void Test34() {
            var targetObj = new SomeConfiguration();
            OptionsDeserializer.Deserialize(targetObj, new string[] { "--initializedBoolList", "false", "--initializedBoolList", "true" });
            Assert.False(targetObj.initializedBoolList[2]);
            Assert.True(targetObj.initializedBoolList[3]);
        }
    }
}
