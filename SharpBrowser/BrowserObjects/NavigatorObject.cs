using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBrowser.BrowserObjects
{
    public class NavigatorObject
    {
        public string UserAgent { get; set; } = ".NET/Console App";
        public string Language { get; set; } = "en-US";
        public bool OnLine { get; set; } = true;

        public string Platform => Environment.OSVersion.Platform.ToString();
        public string AppVersion => Environment.Version.ToString();

        public void PrintStatus()
        {
            Console.WriteLine($"[NAVIGATOR] UserAgent: {UserAgent}, Language: {Language}, Online: {OnLine}");
        }
    }
}
