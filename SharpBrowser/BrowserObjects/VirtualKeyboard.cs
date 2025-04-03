namespace SharpBrowser.BrowserObjects
{
    public class VirtualKeyboard
    {
        public DOMRect BoundingRect { get; set; } = new DOMRect();
        public Action OnGeometryChange { get; set; }
        public bool OverlaysContent { get; set; } = false;
    }
}
