using AngleSharp;
using AngleSharp.Dom;
using Jint;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class HTMLDocument : HTMLElement
    {
        public HTMLElement Head { get; private set; }
        public HTMLElement Body { get; private set; }
        public string Title { get => title; }
        public string Cookie { get => cookie; }

        public IEnumerable<HTMLElement> Styles => GetElementsByTagName("style");

        private string title = string.Empty;
        private string cookie = string.Empty;

        public HTMLDocument(Engine engine) : base(engine, "html")
        {
            this.AddMethod("getElementById", (args) =>
            {
                if (args.Length == 0 || !args[0].IsString()) return Undefined;
                var id = args[0].AsString();
                var result = GetElementById(id);
                return FromObject(Engine, result);
            });
            this.AddMethod("createElement", args => new HTMLElement(engine, args[0].AsString()));

            this.FastSetProperty("title", new PropertyDescriptor(title, true, true, true));
            this.FastSetProperty("cookie", new PropertyDescriptor(cookie, true, true, true));
        }

        public void SetHTMLContent(string html)
        {
            this.InnerHTML = string.Empty;
            this.TextContent = string.Empty;
            this.Attributes.Clear();
            this.Children.Clear();
            this.Style.Clear();
            this.title = string.Empty;
            this.cookie = string.Empty;

            var context = BrowsingContext.New(Configuration.Default);
            var doc = context.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();

            var htmlNode = doc.DocumentElement;
            foreach (var child in htmlNode.Children)
            {
                var el = ConvertAngleNode(child, Engine);
                this.AppendChild(el);
            }

            Head = this.Children.FirstOrDefault(e => e.TagName == "HEAD");
            Body = this.Children.FirstOrDefault(e => e.TagName == "BODY");
            title = doc.Title ?? "";
            this.TextContent = htmlNode.Text();
            this.InnerHTML = htmlNode.InnerHtml;

            this.FastSetProperty("head", new PropertyDescriptor(Head, true, true, true));
            this.FastSetProperty("body", new PropertyDescriptor(Body, true, true, true));
            this.FastSetProperty("title", new PropertyDescriptor(title, true, true, true));
            this.FastSetProperty("cookie", new PropertyDescriptor(cookie, true, true, true));
        }

        private HTMLElement GetElementById(string id)
        {
            return FindById(this, id);
        }

        private static HTMLElement FindById(HTMLElement el, string id)
        {
            if (el.Attributes.TryGetValue("id", out var val) && val == id)
                return el;

            foreach (var child in el.Children)
            {
                var found = FindById(child, id);
                if (found != null)
                    return found;
            }
            return null;
        }

        private static HTMLElement ConvertAngleNode(IElement node, Engine engine)
        {
            var element = new HTMLElement(engine, node.TagName.ToLowerInvariant())
            {
                InnerHTML = node.InnerHtml,
                TextContent = node.Text()
            };

            foreach (var attr in node.Attributes)
            {
                element.Attributes[attr.Name] = attr.Value;
            }

            // Optional: parse style attribute into dictionary
            if (element.Attributes.TryGetValue("style", out var styleStr))
            {
                foreach (var part in styleStr.Split(';'))
                {
                    var pair = part.Split(':');
                    if (pair.Length == 2)
                        element.Style[pair[0].Trim()] = pair[1].Trim();
                }
            }

            foreach (var child in node.Children)
            {
                var childElement = ConvertAngleNode(child, engine);
                element.AppendChild(childElement);
            }

            return element;
        }
    }
}
