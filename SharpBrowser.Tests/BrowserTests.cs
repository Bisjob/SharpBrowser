using Moq;

namespace SharpBrowser.Tests
{
    public class BrowserTests
    {
        private readonly Mock<HttpClient> client;
        private readonly Browser browser;
        public BrowserTests()
        {
            client = new Mock<HttpClient>();
            browser = new Browser(client.Object, new BrowserOptions());
        }

        [Fact]
        public void CanUseConsole()
        {
            var script = 
                "console.log('Hello');" +
                "console.log('Hello', true);" +
                "console.log('Hello', [true]);" +
                "console.log('A', 'B', 3, false);" +
                "console.log('A', ['B', 3, false]);";


            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void ConsoleIsExtensible()
        {
            var script =
                "console.toto = 2;" +
                "console.log(console.toto);";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }
    }
}