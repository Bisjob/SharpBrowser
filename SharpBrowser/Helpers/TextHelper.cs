namespace SharpBrowser.Helpers
{
    internal static class TextHelper
    {
        public static string ToKebabCase(string name)
        {
            return string.Concat(name.Select((c, i) =>
                char.IsUpper(c) ? (i > 0 ? "-" : "") + char.ToLowerInvariant(c) : c.ToString()));
        }
    }
}
