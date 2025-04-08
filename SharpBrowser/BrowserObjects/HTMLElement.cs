using Jint;
using Jint.Native;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;
using SharpBrowser.Helpers;

namespace SharpBrowser.BrowserObjects
{
    public class HTMLElement : ExpandableObject
    {
        public event EventHandler HasChanged;
        public string TagName { get; }
        public string InnerHTML { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
        public Dictionary<string, string> Attributes { get; } = [];
        public Dictionary<string, string> Style { get; } = [];
        public List<HTMLElement> Children { get; } = [];
        public HTMLElement ParentElement { get; private set; }

        private readonly Dictionary<string, List<Delegate>> eventListeners = [];

        public HTMLElement(Engine engine, string tagName) : base(engine)
        {
            TagName = tagName.ToUpperInvariant();

            FastSetProperty("tagName", new PropertyDescriptor(TagName, false, true, true));
            FastSetProperty("innerHTML", new PropertyDescriptor(InnerHTML, true, true, true));
            FastSetProperty("textContent", new PropertyDescriptor(TextContent, true, true, true));

            var styleProxy = new HTMLStyleProxy(engine);
            styleProxy.StyleChanged += (sender, e) =>
            {
                var cssKey = TextHelper.ToKebabCase(e.Name);
                Style[cssKey] = e.Value.AsString();
                Attributes["style"] = string.Join("; ", Style.Select(kv => $"{kv.Key}: {kv.Value}"));
                RaiseHasChanged();
            };
            FastSetProperty("style", new PropertyDescriptor(styleProxy, true, true, true));


            var classList = new ExpandableObject(engine, "classList");
            classList.AddMethod("add", args =>
            {
                foreach (var arg in args)
                {
                    var name = arg.ToString();
                    if (!Attributes.TryGetValue("class", out var classes))
                        classes = "";

                    var list = classes.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!list.Contains(name))
                    {
                        list.Add(name);
                        Attributes["class"] = string.Join(" ", list);
                    }
                }
                RaiseHasChanged();
            });
            classList.AddMethod("remove", args =>
            {
                foreach (var arg in args)
                {
                    var name = arg.ToString();
                    if (Attributes.TryGetValue("class", out var classes))
                    {
                        var list = classes.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                        list.Remove(name);
                        Attributes["class"] = string.Join(" ", list);
                    }
                }
                RaiseHasChanged();
            });
            FastSetProperty("classList", new PropertyDescriptor(classList, true, true, true));


            // Core DOM Methods
            this.AddMethod("appendChild", args =>
            {
                if (args.Length > 0 && args[0].ToObject() is HTMLElement child)
                    AppendChild(child);
            });
            this.AddMethod("setAttribute", args =>
            {
                if (args.Length >= 2)
                    SetAttribute(args[0].AsString(), args[1].AsString());
            });
            this.AddMethod("getAttribute", args =>
            {
                if (args.Length >= 1 && args[0].IsString())
                {
                    var result = GetAttribute(args[0].AsString());
                    return FromObject(Engine, result);
                }
                return Undefined;
            });
            this.AddMethod("removeAttribute", args =>
            {
                if (args.Length >= 1 && args[0].IsString())
                    RemoveAttribute(args[0].AsString());
            });
            this.AddMethod("hasAttribute", args =>
            {
                if (args.Length >= 1 && args[0].IsString())
                {
                    var result = HasAttribute(args[0].AsString());
                    return FromObject(Engine, result);
                }
                return Undefined;
            });

            // Querying
            this.AddMethod("getElementsByTagName", args =>
            {
                if (args.Length == 0 || !args[0].IsString())
                    return FromObject(Engine, Array.Empty<HTMLElement>());

                var result = GetElementsByTagName(args[0].AsString());
                return FromObject(Engine, result);
            });
            this.AddMethod("getElementsByClassName", args =>
            {
                if (args.Length == 0 || !args[0].IsString()) 
                    return FromObject(Engine, Array.Empty<HTMLElement>());

                var cls = args[0].AsString();
                var result = GetElementsByClassName(cls);
                return FromObject(Engine, result);
            });
            this.AddMethod("querySelector", args =>
            {
                if (args.Length == 0 || !args[0].IsString()) 
                    return Undefined;
                var result = QuerySelector(args[0].AsString());
                return FromObject(Engine, result);
            });
            this.AddMethod("querySelectorAll", args =>
            {
                if (args.Length == 0 || !args[0].IsString())
                    return Undefined;
                var result = QuerySelectorAll(args[0].AsString());
                return FromObject(Engine, result);
            });

            // Events
            this.AddMethod("addEventListener", args =>
            {
                if (args.Length >= 2 && args[0].IsString() && args[1].IsFunction())
                {
                    var type = args[0].AsString();
                    var callback = args[1];
                    var cb = (JsValue[] callbackArgs) => callback.Call(callbackArgs);
                    AddEventListener(type, cb);
                }
            });
            this.AddMethod("removeEventListener", args =>
            {
                if (args.Length >= 2  && args[0].IsString() && args[1].IsFunction())
                {
                    var type = args[0].AsString();
                    // Need to take same reference or register a hash to remove it
                    var cb = args[1].ToObject() as Delegate;
                    RemoveEventListener(type, cb);
                }
            });
            this.AddMethod("dispatchEvent", args =>
            {
                if (args.Length >= 1 && args[0].IsString())
                {
                    // can pass real event args if needed
                    DispatchEvent(args[0].AsString());
                }
            });
        }

        
        // === DOM Methods ===
        public void AppendChild(HTMLElement child)
        {
            child.ParentElement = this;
            Children.Add(child);
        }
        public void SetAttribute(string key, string value)
        {
            Attributes[key] = value;
            if (key == "style")
            {
                Style.Clear();
                foreach (var pair in value.Split(';'))
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2)
                        Style[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }
        public string GetAttribute(string key)
        {
            return Attributes.TryGetValue(key, out var value) ? value : null;
        }
        public void RemoveAttribute(string key)
        {
            Attributes.Remove(key);
        }
        public bool HasAttribute(string key)
        {
            return Attributes.ContainsKey(key);
        }

        // === Querying ===
        public IEnumerable<HTMLElement> GetElementsByTagName(string tag)
        {
            tag = tag.ToUpperInvariant();
            var list = new List<HTMLElement>();
            CollectBy(this, list, el => el.TagName.Equals(tag, StringComparison.OrdinalIgnoreCase));
            return list;
        }
        public IEnumerable<HTMLElement> GetElementsByClassName(string cls)
        {
            var list = new List<HTMLElement>();
            CollectBy(this, list, el =>
                el.Attributes.TryGetValue("class", out var val) &&
                val.Split(' ').Contains(cls));
            return list;
        }
        public HTMLElement QuerySelector(string selector)
            => Query(this, selector);
        public IEnumerable<HTMLElement> QuerySelectorAll(string selector)
        {
            var list = new List<HTMLElement>();
            CollectBy(this, list, el => Matches(el, selector));
            return list;
        }

        private static void CollectBy(HTMLElement el, List<HTMLElement> acc, Func<HTMLElement, bool> pred)
        {
            foreach (var child in el.Children)
            {
                if (pred(child)) acc.Add(child);
                CollectBy(child, acc, pred);
            }
        }

        private static HTMLElement Query(HTMLElement el, string selector)
        {
            foreach (var child in el.Children)
            {
                if (Matches(child, selector))
                    return child;

                var found = Query(child, selector);
                if (found != null)
                    return found;
            }
            return null;
        }

        private static bool Matches(HTMLElement el, string selector)
        {
            var parts = selector.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
            return MatchesRecursive(el, parts, 0);
        }

        private static bool MatchesRecursive(HTMLElement el, string[] parts, int index)
        {
            if (el == null || index >= parts.Length)
                return false;

            if (!MatchesSingle(el, parts[index]))
                return false;

            if (index == parts.Length - 1)
                return true;

            return MatchesAncestor(el.ParentElement, parts, index + 1);
        }

        private static bool MatchesAncestor(HTMLElement el, string[] parts, int index)
        {
            while (el != null)
            {
                if (MatchesRecursive(el, parts, index))
                    return true;

                el = el.ParentElement;
            }
            return false;
        }

        private static bool MatchesSingle(HTMLElement el, string selector)
        {
            if (selector.StartsWith('.'))
            {
                var cls = selector[1..];
                return el.Attributes.TryGetValue("class", out var val) && val.Split(' ').Contains(cls);
            }
            if (selector.StartsWith('#'))
            {
                var id = selector[1..];
                return el.Attributes.TryGetValue("id", out var val) && val == id;
            }

            // Match tag name
            return string.Equals(el.TagName, selector, StringComparison.OrdinalIgnoreCase);
        }

        // === Events ===
        public void AddEventListener(string type, Delegate cb)
        {
            if (!eventListeners.ContainsKey(type))
                eventListeners[type] = [];
            eventListeners[type].Add(cb);
        }
        public void RemoveEventListener(string type, Delegate cb)
        {
            if (eventListeners.TryGetValue(type, out var list))
                list.Remove(cb);
        }
        public void DispatchEvent(string type, params object[] args)
        {
            if (eventListeners.TryGetValue(type, out var list))
            {
                foreach (var cb in list)
                    cb.DynamicInvoke(new object[] { args });
            }
        }

        public override string ToString()
        {
            var attrs = string.Join(" ", Attributes.Select(kv => $"{kv.Key}=\"{kv.Value}\""));
            string result = $"<{TagName.ToLowerInvariant()} {attrs}>";
            if (Children.Count == 0)
                result += InnerHTML;
            else
                foreach (var child in Children)
                    result += child.ToString();

            result += $"</{TagName.ToLowerInvariant()}>";
            return result;
        }

        internal void RaiseHasChanged()
        {
            HasChanged?.Invoke(this, EventArgs.Empty);
            ParentElement?.RaiseHasChanged();
        }
    }
}
