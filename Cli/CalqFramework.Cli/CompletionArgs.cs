namespace CalqFramework.Cli {

    /// <summary>
    /// Arguments for completion requests.
    /// </summary>
    internal class CompletionArgs {
        public int Position { get; set; }
        public string Words { get; set; } = "";
    }
}
