using Jint;
using Jint.Native;
using Jint.Native.Function;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SharpBrowser.Extensions
{
    internal static class JsValueExtensions
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFunction(this JsValue value)
        {
            return value.IsObject() && value.AsObject() is Function;
        }

        public static bool IsValidJson(this JsValue value, out JsonDocument json)
        {
            try
            {
                json = JsonDocument.Parse(value.ToString());
                return true;
            }
            catch (JsonException)
            {
                json = null;
                return false;
            }
        }
    }
}
