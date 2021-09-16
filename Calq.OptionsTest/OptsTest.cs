using Ghbvft6.Calq.Options;
using System;
using Xunit;

namespace Ghbvft6.Calq.OptionsTest
{
    public class OptsTest
    {
        [Fact]
        public void Test1()
        {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "--boolean" });
            Assert.True(instance.boolean);
        }

        [Fact]
        public void Test2() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "-b" });
            Assert.True(instance.boolean);
        }

        [Fact]
        public void Test3() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "+x" });
            Assert.False(instance.xtrueBoolean);
        }

        [Fact]
        public void Test4() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "-bx" });
            Assert.True(instance.boolean);
            Assert.True(instance.xtrueBoolean);
        }

        [Fact]
        public void Test5() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "--integer=10", "--text=abc xyz"});
            Assert.Equal(10, instance.integer);
            Assert.Equal("abc xyz", instance.text);
        }

        [Fact]
        public void Test6() {
            var instance = new TestConfiguration();
            var index = Opts.Load(instance, new string[] { "--integer=10", "--", "--text=abc xyz" });
            Assert.Equal(10, instance.integer);
            Assert.Equal(2, index);
            Assert.Null(instance.text);
        }

        [Fact]
        public void Test7() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "-bi" });
            });
            Assert.Equal("not all stacked options are boolean: bi", ex.Message);
        }

        [Fact]
        public void Test8() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "-ib" });
            });
            Assert.Equal("option requires value: -ib", ex.Message);
        }

        [Fact]
        public void Test9() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "-ib", "0" });
            });
            Assert.Equal("not all stacked options are boolean: ib", ex.Message);
        }

        [Fact]
        public void Test10() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--integer" });
            });
            Assert.Equal("option requires value: --integer", ex.Message);
        }

        [Fact]
        public void Test11() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "++xtrueBoolean" });
            Assert.False(instance.xtrueBoolean);
        }

        [Fact]
        public void Test12() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--inner=0" });
            });
            Assert.Equal($"option and value type mismatch: inner=0 (inner is Inner)", ex.Message);
        }

        [Fact]
        public void Test13() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--integer=a" });
            });
            Assert.Equal($"option and value type mismatch: integer=a (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test14() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--integer=0.1" });
            });
            Assert.Equal($"option and value type mismatch: integer=0.1 (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test15() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--aByteNumber=256" });
            });
            Assert.Equal($"option value is out of range: aByteNumber=256 (0-255)", ex.Message);
        }

        [Fact]
        public void Test16() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "--missingMemmber=0" });
            });
            Assert.Equal($"option doesn't exist: missingMemmber", ex.Message);
        }

        [Fact]
        public void Test17() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(instance, new string[] { "-m" });
            });
            Assert.Equal($"option doesn't exist: m", ex.Message);
        }

        [Fact]
        public void Test18() {
            var instance = new TestConfiguration();
            Opts.Load(instance, new string[] { "-b", "-x" });
            Assert.True(instance.boolean);
            Assert.True(instance.xtrueBoolean);
        }

        [Fact]
        public void Test19() {
            var instance = new TestConfiguration();
            var index = Opts.Load(instance, new string[] { "--integer=10", "notanoption", "--text=abc xyz" });
            Assert.Equal(10, instance.integer);
            Assert.Equal(1, index);
            Assert.Null(instance.text);
        }

        [Fact]
        public void Test20() {
            var instance = new TestConfiguration();
            var index = Opts.Load(instance, new string[] { "--integer", "10", "notanoption", "--text=abc xyz" });
            Assert.Equal(10, instance.integer);
            Assert.Equal(2, index);
            Assert.Null(instance.text);
        }
    }
}
