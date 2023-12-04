#pragma warning disable CS0649

namespace CalqFramework.OptionsTest {
    class SomeTool {

        public class Inner {
            public void InnerFoo() { }
        }

        public string text;
        public int integer;

        public Inner inner = new Inner();
        public Inner nullInner;

        public void Foo() { }

        public void FooWithOptionalParam(bool optional = true) { }

        public string Text(string text) {
            return text;
        }

        public int Integer(int integer) {
            return integer;
        }

        public void TextAndInteger(string text, int integer) {
            this.text = text;
            this.integer = integer;
        }

        public void IntegerAndText(int integer, string text) {
            this.integer = integer;
            this.text = text;
        }
    }
}
