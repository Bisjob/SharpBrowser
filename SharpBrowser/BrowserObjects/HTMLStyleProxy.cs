using Jint;
using Jint.Native;

namespace SharpBrowser.BrowserObjects
{
    public class StyleChangedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public JsValue Value { get; set; }
    }


    public class HTMLStyleProxy : ExpandableObject
    {
        public event EventHandler<StyleChangedEventArgs> StyleChanged;
        public HTMLStyleProxy(Engine engine) : base(engine, "Style")
        {

        }

        public override bool Set(JsValue property, JsValue value, JsValue receiver)
        {
            bool ok = base.Set(property, value, receiver);
            if (ok)
                StyleChanged?.Invoke(this, new StyleChangedEventArgs() { Name = property.ToString(), Value = value });
            return ok;
        }
    }
}
