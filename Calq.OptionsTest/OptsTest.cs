using Calq.Options;
using System;
using Xunit;

namespace Calq.OptionsTest
{
    public class OptsTest
    {
        [Fact]
        public void Test1()
        {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "--boolean" }, instance);
            Assert.True(instance.boolean);
        }

        [Fact]
        public void Test2() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "-b" }, instance);
            Assert.True(instance.boolean);
        }

        [Fact]
        public void Test3() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "+x" }, instance);
            Assert.False(instance.xtrueBoolean);
        }

        [Fact]
        public void Test4() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "-bx" }, instance);
            Assert.True(instance.boolean);
            Assert.True(instance.xtrueBoolean);
        }

        [Fact]
        public void Test5() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "--integer=10", "--text=abc xyz"}, instance);
            Assert.Equal(10, instance.integer);
            Assert.Equal("abc xyz", instance.text);
        }

        [Fact]
        public void Test6() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "--integer=10", "--", "--text=abc xyz" }, instance);
            Assert.Equal(10, instance.integer);
            Assert.Null(instance.text);
        }

        [Fact]
        public void Test7() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "-bi" }, instance);
            });
            Assert.Equal("not all stacked options are boolean: bi", ex.Message);
        }

        [Fact]
        public void Test8() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "-ib" }, instance);
            });
            Assert.Equal("option requires value: -ib", ex.Message);
        }

        [Fact]
        public void Test9() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "-ib", "0" }, instance);
            });
            Assert.Equal("not all stacked options are boolean: ib", ex.Message);
        }

        [Fact]
        public void Test10() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--integer" }, instance);
            });
            Assert.Equal("option requires value: --integer", ex.Message);
        }

        [Fact]
        public void Test11() {
            var instance = new TestConfiguration();
            Opts.Load(new string[] { "++xtrueBoolean" }, instance);
            Assert.False(instance.xtrueBoolean);
        }

        [Fact]
        public void Test12() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--inner=0" }, instance);
            });
            Assert.Equal($"option and value type mismatch: inner=0 (inner is Inner)", ex.Message);
        }

        [Fact]
        public void Test13() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--integer=a" }, instance);
            });
            Assert.Equal($"option and value type mismatch: integer=a (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test14() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--integer=0.1" }, instance);
            });
            Assert.Equal($"option and value type mismatch: integer=0.1 (integer is Int32)", ex.Message);
        }

        [Fact]
        public void Test15() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--aByteNumber=256" }, instance);
            });
            Assert.Equal($"option value is out of range: aByteNumber=256 (0-255)", ex.Message);
        }

        [Fact]
        public void Test16() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "--missingMemmber=0" }, instance);
            });
            Assert.Equal($"option doesn't exist: missingMemmber", ex.Message);
        }

        [Fact]
        public void Test17() {
            var ex = Assert.Throws<Exception>(() => {
                var instance = new TestConfiguration();
                Opts.Load(new string[] { "-m" }, instance);
            });
            Assert.Equal($"option doesn't exist: m", ex.Message);
        }
    }
}
