using Jint;
using Jint.Native;
using System;

namespace SharpBrowser.Playground
{
    internal class Program
    {
        static Browser browser;
        static HttpClient client;
        static async Task Main(string[] args)
        {
            client = new HttpClient();
            browser = new Browser(client, new BrowserOptions());

            var script = File.ReadAllText("test.js");
            //var script = "window.setTimeout(function () {console.log(\"windows.setTimeout callback executed\");}, 1000);";
            var result = browser.ExecuteScript(script);


            Console.ReadKey();
        }
    }
}
