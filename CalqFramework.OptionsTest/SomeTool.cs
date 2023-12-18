#pragma warning disable CS0649

using System.Collections.Generic;

namespace CalqFramework.OptionsTest {
    class SomeTool {

        public class Inner {
            public void InnerFoo() { }
        }

        public string internalText;
        public int internalInteger;
        public bool internalBoolean;
        public bool theSameBoolean;

        public Inner inner = new Inner();
        public Inner nullInner;

        public List<bool> initializedBoolList = new List<bool>() { true, false };

        public void Foo() { }

        public void FooWithOptionalParam(bool optional = true) { }

        public List<bool> FooWithList(List<bool> paramList) {
            return paramList;
        }

        public string Text(string text) {
            return text;
        }

        public int Integer(int integer) {
            return integer;
        }

        public void TextAndInteger(string text, int integer) {
            this.internalText = text;
            this.internalInteger = integer;
        }

        public void IntegerAndText(int integer, string text) {
            this.internalInteger = integer;
            this.internalText = text;
        }

        public void TextAndBoolean(string text, bool boolean = false) {
            this.internalText = text;
            this.internalBoolean = boolean;
        }

        public void TextAndBooleanError(string text, bool theSameBoolean = false) {
            this.internalText = text;
            this.theSameBoolean = theSameBoolean;
        }
    }
}
