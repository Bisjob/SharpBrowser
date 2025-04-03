namespace SharpBrowser.BrowserObjects
{
    public class ServiceWorkerContainer
    {
        public object Controller { get; set; } = null;
        public Action OnControllerChange { get; set; }
        public Action OnMessage { get; set; }
        public Action OnMessageError { get; set; }
    }
}
