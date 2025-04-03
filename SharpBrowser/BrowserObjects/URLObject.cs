using Jint;
using Jint.Native;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class URLObject : ExpandableObject
    {
        public override bool Extensible => true;

        public URLObject(Engine engine) : base(engine, "URL")
        {
            this.AddMethod("createObjectURL", CreateObjectURL);
            this.AddMethod("revokeObjectURL", RevokeObjectURL);
        }

        private JsValue CreateObjectURL(JsValue[] args)
        {
            Console.WriteLine("URL.createObjectURL called (stub)");
            return FromObject(Engine, "blob:mock-url");
        }
        private void RevokeObjectURL(JsValue[] args)
        {
            Console.WriteLine("URL.revokeObjectURL called (stub)");
        }
    }
}
