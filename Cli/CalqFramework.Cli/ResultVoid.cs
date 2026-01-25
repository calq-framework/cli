namespace CalqFramework.Cli {
    /// <summary>
    /// Represents a void return value for CLI commands that don't return data.
    /// </summary>
    sealed public class ResultVoid {
        public static readonly ResultVoid Value = new ResultVoid();

        private ResultVoid() {
        }
    }
}
