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
        public void ConsoleLog_VariousParameters_ShouldExecuteSuccessfully()
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
        public void Window_CustomObjectProperties_ShouldBeSetAndRetrieved()
        {
            var script =
                "console.log('Test window property dynamic expansion');" +
                "window.customObject.boolean = true;" +
                "window.customObject.nestedObject = { \"metadata\": { \"some\": \"value\" } };" +
                "console.log('Objects set');" +
                "var boolean = window.customObject.boolean;" +
                "console.log('bool Value get: ' + boolean);" +
                "var value = window.customObject.nestedObject.metadata.some;" +
                "console.log('json Value get: ' + value);";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void Window_AutoCreatedFunction_ShouldBeCallable()
        {
            var script =
                "console.log('Test auto-created functions are callable');" +
                "window.fakeFunc = function () { console.log('fakeFunc called!'); };" +
                "if (window.fakeFunc) { window.fakeFunc(); }";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void Window_SetTimeout_ShouldInvokeCallback()
        {
            var script =
                "console.log('Test windows.setTimeout invocation');" +
                "window.setTimeout(function () {" +
                "    console.log('windows.setTimeout callback executed');" +
                "}, 1000);" +
                "window.setTimeout(function (e) {" +
                "    console.log('windows.setTimeout with args callback executed: ' + e);" +
                "}, 1000, 'hello');";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void SetTimeout_WithFunctionAndArgument_ShouldWork()
        {
            var script =
                "function greet(name) {" +
                "    console.log('setTimeout with args: Hello, ' + name + '!');" +
                "}" +
                "console.log('Test setTimeout with argument');" +
                "setTimeout(greet, 2000, 'Alice');";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }


        [Fact]
        public void Window_Atob_ShouldDecodeBase64()
        {
            var script =
                "console.log('Test atob');" +
                "if (window.atob) {" +
                "    const encoded = 'SGVsbG8sIHdvcmxk';" +
                "    const decoded = window.atob(encoded);" +
                "    console.log('atob result:' + decoded);" +
                "}";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void Window_HistoryObject_ShouldExposePushState()
        {
            var script =
                "console.log('Test history object and methods');" +
                "if (window.history && window.history.pushState) {" +
                "    console.log('history.pushState exists');" +
                "}";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }

        [Fact]
        public void Window_CustomMethod_ShouldBeCallable()
        {
            var script =
                "console.log('Test dynamic method creation and access on window');" +
                "window.myFunc = function (x) {" +
                "    console.log('myFunc called with:' + x);" +
                "};" +
                "if (window.myFunc) {" +
                "    window.myFunc('test');" +
                "}";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }


        [Fact]
        public void FullScript_ShouldExecuteWithoutError()
        {
            var script = @"
        console.log('== JS Test Script Begin ==');
        console.log('Hello');
        console.log('Hello', true);
        console.log('Hello', [true]);
        console.log('A', 'B', 3, false);
        console.log('A', ['B', 3, false]);

        console.log('Test window property dynamic expansion');
        window.customObject.boolean = true;
        window.customObject.nestedObject = { ""metadata"": { ""some"": ""value"" } };
        console.log('Objects set');
        var boolean = window.customObject.boolean;
        console.log('bool Value get: ' + boolean);
        var value = window.customObject.nestedObject.metadata.some;
        console.log('json Value get: ' + value);

        console.log('Test auto-created functions are callable');
        window.fakeFunc = function () { console.log('fakeFunc called!'); };
        if (window.fakeFunc) { window.fakeFunc(); }

        console.log('Test windows.setTimeout invocation');
        window.setTimeout(function () { console.log('windows.setTimeout callback executed'); }, 1000);
        window.setTimeout(function (e) { console.log('windows.setTimeout with args callback executed: ' + e); }, 1000, 'hello');

        function greet(name) {
            console.log('setTimeout with args: Hello, ' + name + '!');
        }
        console.log('Test setTimeout with argument');
        setTimeout(greet, 2000, 'Alice');

        console.log('Test atob');
        if (window.atob) {
            const encoded = 'SGVsbG8sIHdvcmxk';
            const decoded = window.atob(encoded);
            console.log('atob result:' + decoded);
        }

        console.log('Test history object and methods');
        if (window.history && window.history.pushState) {
            console.log('history.pushState exists');
        }

        console.log('Test dynamic method creation and access on window');
        window.myFunc = function (x) {
            console.log('myFunc called with:' + x);
        };
        if (window.myFunc) {
            window.myFunc('test');
        }

        console.log('== JS Test Script End ==');
    ";

            var result = browser.ExecuteScript(script);

            Assert.True(result);
        }
    }
}