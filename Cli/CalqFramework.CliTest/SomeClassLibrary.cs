#pragma warning disable CS0649

using System.Collections.Generic;

namespace CalqFramework.CliTest {

    /// <summary>
    /// Some summary.
    /// </summary>
    class SomeClassLibrary {

        public class Nested {
            public void NestedMethod() { }
        }

        public string textField;
        public int integerField;
        public bool booleanField;
        public bool booleanConflict;

        public Nested objectField = new Nested();
        public Nested nullObjectField;

        public List<bool> initializedBoolList = new List<bool>() { true, false };

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Method() { }

        public void MethodWithOptionalParam(bool optional = true) { }

        public List<bool> MethodWithList(List<bool> paramList) {
            return paramList;
        }

        public string MethodWithText(string text) {
            return text;
        }

        public int MethodWithInteger(int integer) {
            return integer;
        }

        public void MethodWithTextAndInteger(string text, int integer) {
            this.textField = text;
            this.integerField = integer;
        }

        public void MethodWithIntegerAndText(int integer, string text) {
            this.integerField = integer;
            this.textField = text;
        }

        public void MethodWithTextAndBoolean(string text, bool boolean = false) {
            this.textField = text;
            this.booleanField = boolean;
        }

        public void MethodWithTextAndBooleanError(string text, bool booleanConflict = false) {
            this.textField = text;
            this.booleanConflict = booleanConflict;
        }
    }
}
