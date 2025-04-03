using Jint;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class ConsoleObject : ExpandableObject
    {
        public ConsoleObject(Engine engine) : base(engine, "Console")
        {
            AddLogMethod("log", Console.WriteLine);
            AddLogMethod("warn", msg => Console.WriteLine("[WARN] " + msg));
            AddLogMethod("error", msg => Console.WriteLine("[ERROR] " + msg));
            AddLogMethod("debug", msg => Console.WriteLine("[DEBUG] " + msg));
            AddLogMethod("info", msg => Console.WriteLine("[INFO] " + msg));
        }
        private void AddLogMethod(string name, Action<string> action)
        {
            void method(object[] args)
            {
                var output = args != null
                    ? string.Join(" ", args.Select(a => a.ToString()))
                    : "";
                action("[INTERNAL] " + output);
            }
            this.AddMethod(name, method);
        }

    }
}
