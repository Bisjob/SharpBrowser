using Jint;
using Jint.Native;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class History : ExpandableObject
    {
        private readonly List<HistoryEntry> entries = new();
        private int currentIndex = -1;

        public History(Engine engine) : base(engine, "Hisotry")
        {
            this.AddMethod("pushState", args =>
            {
                if (args == null)
                    return Undefined;

                var state = args.Length > 0 ? args[0] : null;
                var url = args.Length > 2 ? args[2].ToString() : "";
                PushState(url, state);

                return Undefined;
            });
            this.AddMethod("replaceState", args =>
            {
                if (args == null)
                    return true;

                args ??= Array.Empty<JsValue>();
                var state = args.Length > 0 ? args[0] : null;
                var url = args.Length > 2 ? args[2].ToString() : "";
                return ReplaceState(url, state);
            });
            this.AddMethod("back", args => Back());
            this.AddMethod("forward", args => Forward());
            this.AddMethod("go", args =>
            {
                if (args.Length > 0 && args[0].IsNumber())
                    Go((int)args[0].AsNumber());
            });

            FastSetProperty("length", new PropertyDescriptor(0, true, true, true));
            FastSetProperty("state", new PropertyDescriptor(Null, true, true, true));
        }

        public void PushState(string url, object state)
        {
            entries.RemoveRange(currentIndex + 1, entries.Count - currentIndex - 1);
            entries.Add(new HistoryEntry(state, url));
            currentIndex++;

            UpdateProps();
        }

        public bool ReplaceState(string url, object state)
        {
            if (string.IsNullOrEmpty(url) || state == null)
                return true;

            if (currentIndex >= 0 && currentIndex < entries.Count)
            {
                entries[currentIndex] = new HistoryEntry(state, url);
            }
            else
            {
                entries.Add(new HistoryEntry(state, url));
                currentIndex = entries.Count - 1;
            }

            UpdateProps();

            return true;
        }

        public void Back()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                UpdateProps();
            }
        }

        public void Forward()
        {
            if (currentIndex < entries.Count - 1)
            {
                currentIndex++;
                UpdateProps();
            }
        }

        private void Go(int delta)
        {
            int newIndex = currentIndex + delta;
            if (newIndex >= 0 && newIndex < entries.Count)
            {
                currentIndex = newIndex;
                UpdateProps();
            }
        }

        private void UpdateProps()
        {
            var current = currentIndex >= 0 && currentIndex < entries.Count ? entries[currentIndex] : null;

            FastSetProperty("length", new PropertyDescriptor(entries.Count, true, true, true));
            FastSetProperty("state", new PropertyDescriptor(FromObject(Engine, current?.State), true, true, true));

            if (current != null)
            {
                Console.WriteLine($"[History] Navigated to URL: {current.Url}");
            }
        }
    }

    internal class HistoryEntry
    {
        public object State { get; set; }
        public string Url { get; set; }

        public HistoryEntry(object state, string url)
        {
            State = state;
            Url = url;
        }
    }
}
