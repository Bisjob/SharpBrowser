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
            browser.Options.AutoExecuteScript = true;



            //var script = File.ReadAllText("test.js");
            ////var script = "window.setTimeout(function () {console.log(\"windows.setTimeout callback executed\");}, 1000);";
            //var result = browser.ExecuteScript(script);

            var firstContent = File.ReadAllText("first.txt");
            await browser.SetHtmlContent(firstContent);

            Console.WriteLine();
            Console.WriteLine("First script executed");
            Console.WriteLine();

            var secondContent = File.ReadAllText("second.txt");
            var result = browser.ExecuteScript(secondContent);

            void WriteProperties(JsValue obj, string tabs = "")
            {
                foreach (var props in obj.AsObject().GetOwnProperties())
                {

                    if (props.Value.Value.IsObject())
                    {
                        Console.WriteLine(tabs + $"{props.Key}:");
                        WriteProperties(props.Value.Value, tabs + "\t");
                    }
                    else
                    {
                        Console.WriteLine(tabs + $"{props.Key}: {props.Value.Value.ToString()}");
                    }
                }
            }
            ;

            WriteProperties(browser.Window);

            Console.ReadKey();
        }
    }
}
