using Jint;
using Jint.Native;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class LocationObject : ExpandableObject
    {
        public Uri CurrentUri { get; private set; }

        public LocationObject(Engine engine, string startUrl = null) : base(engine, "Location")
        {
            if (string.IsNullOrEmpty(startUrl))
                startUrl = "http://localhost";

            SetUrl(startUrl);

            this.AddMethod("querySelectorAll", Replace);
        }

        public void SetUrl(string url)
        {
            CurrentUri = new Uri(url);

            FastSetProperty("href", new PropertyDescriptor(url, true, true, true));
            FastSetProperty("protocol", new PropertyDescriptor(CurrentUri.Scheme + ":", true, true, true));
            FastSetProperty("host", new PropertyDescriptor(CurrentUri.Authority, true, true, true));
            FastSetProperty("hostname", new PropertyDescriptor(CurrentUri.Host, true, true, true));
            FastSetProperty("port", new PropertyDescriptor(CurrentUri.Port.ToString(), true, true, true));
            FastSetProperty("pathname", new PropertyDescriptor(CurrentUri.AbsolutePath, true, true, true));
            FastSetProperty("search", new PropertyDescriptor(CurrentUri.Query, true, true, true));
            FastSetProperty("hash", new PropertyDescriptor(CurrentUri.Fragment, true, true, true));
        }


        private JsValue Replace(JsValue[] args)
        {
            if (args.Length > 0 && args[0].IsString())
            {
                var newHref = args[0].AsString();
                Console.WriteLine($"[location.replace] Replacing URL to: {newHref}");
                // Update internal state
                SetUrl(newHref);
            }

            return Undefined;
        }

    }
}
