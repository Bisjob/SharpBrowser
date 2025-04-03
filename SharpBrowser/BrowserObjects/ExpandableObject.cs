using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using SharpBrowser.Extensions;

namespace SharpBrowser.BrowserObjects
{
    public class ExpandableObject : ObjectInstance
    {
        private readonly string objectName;

        public override bool Extensible => true;

        public ExpandableObject(Engine engine, string name = "") : base(engine)
        {
            objectName = name;
        }

        public override JsValue Get(JsValue property, JsValue receiver)
        {
            var name = property.ToString();
            if (!HasOwnProperty(name))
            {
                var child = new ExpandableObject(Engine, name);
                SetOwnProperty(name, new PropertyDescriptor(child, true, true, true));
                HandleNewProperty(name, child);
#if DEBUG
                Console.WriteLine($"[GET auto-expand] {objectName}.{name}");
#endif
                return child;
            }

            return GetOwnProperty(name).Value;
        }

        public override bool Set(JsValue property, JsValue value, JsValue receiver)
        {
            var name = property.ToString();

            if (value.IsObject() && !value.IsFunction())
            {
                var obj = value.AsObject();
                var newObj = new ExpandableObject(Engine, name);
                foreach (var kv in obj.GetOwnProperties())
                {
                    newObj.Set(kv.Key, kv.Value.Value, newObj);
                }

                SetOwnProperty(name, new PropertyDescriptor(newObj, true, true, true));
                HandleNewProperty(name, newObj);
            }
            else
            {
                SetOwnProperty(name, new PropertyDescriptor(value, true, true, true));
            }

#if DEBUG
            Console.WriteLine($"[SET] {objectName}.{name} = {value}");
#endif
            return true;
        }

        protected virtual void HandleNewProperty(string name, JsValue value) { }

        public override string ToString() => $"[ExpandableObject {objectName}]";
    }
}
