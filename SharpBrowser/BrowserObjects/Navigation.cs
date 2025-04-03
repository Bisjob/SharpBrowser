namespace SharpBrowser.BrowserObjects
{
    public class Navigation
    {
        public bool CanGoBack { get; set; } = false;
        public bool CanGoForward { get; set; } = false;
        public NavigationHistoryEntry CurrentEntry { get; set; } = new NavigationHistoryEntry();
        public Action OnNavigate { get; set; }
        public Action OnCurrentEntryChange { get; set; }
        public Action OnNavigateError { get; set; }
        public Action OnNavigateSuccess { get; set; }
        public object Transition { get; set; } = null;
    }
}
