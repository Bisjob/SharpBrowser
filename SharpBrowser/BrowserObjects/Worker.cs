using Jint;
using Jint.Native.Object;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class Worker : ExpandableObject
    {
        public Worker(Engine engine) : base(engine, "Worker")
        {
            this.AddMethod("postMessage", PostMessage);
        }

        public override bool Extensible => true;

        private void PostMessage(object[] args)
        {
            Console.WriteLine("Worker.postMessage called (stub)");
        }
    }
}
