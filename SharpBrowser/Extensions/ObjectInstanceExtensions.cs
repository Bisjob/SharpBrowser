using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace SharpBrowser.Extensions
{
    internal static class ObjectInstanceExtensions
    {
        internal static void AddMethod(this ObjectInstance obj, string name, Func<JsValue[], JsValue> logic)
        {
            Func<JsValue, JsValue[], JsValue> delegateWrapper = (thisObj, args) =>
            {
                return logic(args);
            };

            var descr = new PropertyDescriptor(new ClrFunction(obj.Engine, name, delegateWrapper), writable: false, enumerable: false, configurable: true);
            obj.FastSetProperty(name, descr);
        }
        internal static void AddMethod(this ObjectInstance obj, string name, Action<JsValue[]> logic)
        {
            Func<JsValue, JsValue[], JsValue> delegateWrapper = (thisObj, args) =>
            {
                logic(args);
                return JsValue.Undefined;
            };

            var descr = new PropertyDescriptor(new ClrFunction(obj.Engine, name, delegateWrapper), writable: false, enumerable: false, configurable: true);
            obj.FastSetProperty(name, descr);
        }
    }
}
