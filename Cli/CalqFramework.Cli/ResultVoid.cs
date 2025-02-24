namespace CalqFramework.Cli {
    sealed public class ResultVoid {
        public static readonly ResultVoid Value = new ResultVoid();

        private ResultVoid() {
        }
    }
}
