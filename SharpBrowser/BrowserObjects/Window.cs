using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;
using System.Text;

namespace SharpBrowser.BrowserObjects
{
    public class Window : ExpandableObject
    {
        public HTMLDocument Document { get; }
        public ClientNavigator Navigator { get; }
        public History History { get; }
        public Worker Worker { get; }
        public URLObject URLObject { get; }
        public Performance Performance { get; }
        public ConsoleObject ConsoleObj { get; }
        public LocationObject Location { get; }

        public double DevicePixelRatio { get; }
        public int InnerHeight { get; }
        public int InnerWidth { get; }
        public int OuterHeight { get; }
        public int OuterWidth { get; }
        public bool OriginAgentCluster { get; }
        public bool IsSecureContext { get; }
        public int Orientation { get; }

        private int timerId = 0;
        private readonly Dictionary<int, Timer> Timers = [];

        public Window(Engine engine, string userAgent) : base(engine, "windows")
        {
            Document = new HTMLDocument(engine);
            Navigator = new ClientNavigator(userAgent);
            History = new History(engine);
            Worker = new Worker(engine);
            URLObject = new URLObject(engine);
            Performance = new Performance(engine);
            ConsoleObj = new ConsoleObject(engine);
            Location = new LocationObject(engine);
            DevicePixelRatio = engine.Evaluate("Math.random() * (1.9 - 1.2) + 1.2").AsNumber();
            InnerHeight = 65;
            InnerWidth = 300;
            OuterHeight = 987;
            OuterWidth = 524;
            OriginAgentCluster = false;
            IsSecureContext = true;
            Orientation = 0;

            AddProperty("history", History);
            AddProperty("document", Document);
            AddProperty("navigator", FromObject(engine, Navigator.ClientInformation));
            AddProperty("performance", FromObject(engine, Performance));
            AddProperty("devicePixelRatio", DevicePixelRatio);
            AddProperty("innerHeight", InnerHeight);
            AddProperty("innerWidth", InnerWidth);
            AddProperty("outerHeight", OuterHeight);
            AddProperty("outerWidth", OuterWidth);
            AddProperty("originAgentCluster", OriginAgentCluster);
            AddProperty("isSecureContext", IsSecureContext);
            AddProperty("orientation", Orientation);
            AddProperty("Worker", Worker);
            AddProperty("URL", URLObject);
            AddProperty("console", ConsoleObj);
            AddProperty("document", Document);
            AddProperty("location", Location);

            // Add base64 atob method
            Func<JsValue[], JsValue> atobDelegate = (args) =>
            {
                if (args == null || args.Length == 0 || !args[0].IsString())
                    return Undefined;

                var result = Atob(args[0].AsString());
                return FromObject(engine, result);
            };
            this.AddMethod("atob", atobDelegate);
            Engine.SetValue("atob", Get("atob"));


            Func<JsValue[], JsValue> timeoutDelegate = (args) =>
            {
                if (args == null || args.Length < 2 || !args[0].IsFunction() || !args[1].IsNumber())
                    return Undefined;

                var callback = args[0];
                var time = args[1];
                var callbackArgs = args.Skip(2).ToArray();
                return SetTimeout(() => callback.Call(callbackArgs), (int)time.AsNumber());
            };
            this.AddMethod("setTimeout", timeoutDelegate);
            Engine.SetValue("setTimeout", Get("setTimeout"));

            Func<JsValue[], JsValue> delayDelegate = (args) =>
            {
                if (args == null || args.Length != 1 || !args[0].IsNumber())
                    return Undefined;
                ClearTimeout((int)args[0].AsNumber());
                return Undefined;
            };
            this.AddMethod("clearTimeout", delayDelegate);
            Engine.SetValue("clearTimeout", Get("clearTimeout"));

            Func<JsValue[], JsValue> intervalDelegate = (args) =>
            {
                if (args == null || args.Length < 2 || !args[0].IsFunction() || !args[1].IsNumber())
                    return Undefined;

                var callback = args[0];
                var time = args[1];
                var callbackArgs = args.Skip(2).ToArray();
                return SetInterval(() => callback.Call(callbackArgs), (int)time.AsNumber());
            };
            this.AddMethod("setInterval", intervalDelegate);
            Engine.SetValue("setInterval", Get("setInterval"));

            Func<JsValue[], JsValue> clearIntervalDelegate = (args) =>
            {
                if (args == null || args.Length != 1 || !args[0].IsNumber())
                    return Undefined;

                ClearInterval((int)args[0].AsNumber());
                return Undefined;
            };
            this.AddMethod("clearInterval", intervalDelegate);
            Engine.SetValue("clearInterval", Get("clearInterval"));

            this.AddMethod("alert", args =>
            {
                Console.WriteLine("[alert] " + string.Join(" ", args.Select(a => a.ToString())));
            });
            Engine.SetValue("alert", Get("alert"));



            Engine.SetValue("window", this);
            Engine.SetValue("self", this);
        }

        public string Atob(string input)
            => Encoding.Latin1.GetString(Convert.FromBase64String(input));

        public int SetTimeout(Delegate callback, int delay, params object[] args)
        {
            if (delay == 0)
            {
                try
                {
                    callback.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Timer callback failed: {e.Message}");
                    Console.ResetColor();
                }
                return -1;
            }

            int id = Interlocked.Increment(ref timerId);
            Timer timer = null;
            timer = new Timer(_ =>
            {
                try
                {
                    callback.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Timer callback failed: {e.Message}");
                    Console.ResetColor();
                }
                timer.Dispose();
                lock (Timers)
                {
                    Timers.Remove(id);
                }
            }, null, delay, Timeout.Infinite);

            lock (Timers)
            {
                Timers.Add(id, timer);
            }
            return id;
        }
        public void ClearTimeout(int id)
        {
            lock (Timers)
            {
                if (Timers.TryGetValue(id, out Timer timer))
                {
                    timer.Dispose();
                    Timers.Remove(id);
                }
            }
        }

        public int SetInterval(Delegate callback, int interval, params object[] args)
        {
            int id = Interlocked.Increment(ref timerId);
            Timer timer = null;
            timer = new Timer(_ =>
            {
                try
                {
                    callback.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Interval callback failed: {e.Message}");
                    Console.ResetColor();
                }
            }, null, interval, interval);

            lock (Timers)
            {
                Timers.Add(id, timer);
            }
            return id;
        }

        public void ClearInterval(int id)
        {
            lock (Timers)
            {
                if (Timers.TryGetValue(id, out Timer timer))
                {
                    timer.Dispose();
                    Timers.Remove(id);
                }
            }
        }


        protected override void HandleNewProperty(string propertyname, JsValue value)
        {
            base.HandleNewProperty(propertyname, value);
            Engine.SetValue(propertyname, value);
        }

        private void AddProperty(string name, JsValue obj)
        {
            FastSetProperty(name, new PropertyDescriptor(obj, true, true, true));
            Engine.SetValue(name, obj);
        }

    }
}
