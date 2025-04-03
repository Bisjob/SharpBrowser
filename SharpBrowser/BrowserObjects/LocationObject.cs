using Jint;

namespace SharpBrowser.BrowserObjects
{
    public class LocationObject : ExpandableObject
    {
        public LocationObject(Engine engine) : base(engine, "location")
        {
        }

        public string Href { get; set; } = "http://localhost";
        public string Host => CurrentUri.Host;
        public string Pathname => CurrentUri.AbsolutePath;
        public string Search => CurrentUri.Query;
        public string Hash => CurrentUri.Fragment;
        public Uri CurrentUri => new(Href);

        public void Reload()
        {
            Console.WriteLine("[LOCATION] Reloading: " + Href);
        }

        public void Assign(string url)
        {
            Href = url;
            Console.WriteLine("[LOCATION] Navigated to: " + Href);
        }

        public void Replace(string url)
        {
            Href = url;
            Console.WriteLine("[LOCATION] Replaced with: " + Href);
        }
    }
}
