using Xunit;

namespace CalqFramework.CliTest {

    [CollectionDefinition("Completion Tests", DisableParallelization = true)]
    public class CompletionTestCollection {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [CollectionDefinition("DotnetSuggest Tests", DisableParallelization = true)]
    public class DotnetSuggestTestCollection {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
