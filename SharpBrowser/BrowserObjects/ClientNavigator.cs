namespace SharpBrowser.BrowserObjects
{
    public class ClientNavigator
    {
        public object ClientInformation { get; private set; }

        public ClientNavigator(string userAgent)
        {
            var language = "en-GB";
            var languagesList = new List<string> { "en-GB", "en-US", "en" };

            ClientInformation = new
            {
                AppCodeName = string.IsNullOrEmpty(userAgent) ? "" : userAgent.Split('/')[0],
                AppName = "Netscape",
                AppVersion = string.IsNullOrEmpty(userAgent) ? "" : userAgent.Split("Mozilla/")[1],
                Bluetooth = new BlueTooth(),
                Clipboard = new ClipBoard(),
                Connection = new NetworkInformation(),
                Contacts = new ContactsManager(),
                CookieEnabled = true,
                Credentials = new CredentialsContainer(),
                DeviceMemory = (long)(2 << new Random().Next(6)),
                DevicePosture = new DevicePosture(),
                Geolocation = new Geolocation(),
                Gpu = new GPU(),
                HardwareConcurrency = 8,
                Ink = new Ink(),
                Keyboard = new Keyboard(),
                Language = language,
                Languages = languagesList,
                Locks = new LocksManager(),
                Managed = new NavigatorManagedData(),
                MaxTouchPoints = 5,
                MediaCapabilities = new MediaCapabilities(),
                MediaDevices = new MediaDevices(),
                MediaSession = new MediaSession(),
                MimeTypes = new MimeTypeArray(),
                Ml = new ML(),
                OnLine = true,
                PdfViewerEnabled = false,
                Permissions = new Permissions(),
                Platform = "Linux x86_64",
                Plugins = new PluginArray(),
                Presentation = new Presentation(),
                Product = "Gecko",
                ProductSub = "20030107",
                Scheduling = new Scheduling(),
                ServiceWorker = new ServiceWorkerContainer(),
                Storage = new StorageManager(),
                StorageBuckets = new StorageBucketManager(),
                Usb = new USB(),
                UserActivation = new UserActivation(),
                UserAgent = userAgent,
                UserAgentData = new NavigatorUAData(),
                Vendor = "Google Inc.",
                VendorSub = "",
                VirtualKeyboard = new VirtualKeyboard(),
                WakeLock = new WakeLock(),
                Webdriver = false,
                WebkitPersistentStorage = new DeprecatedStorageQuota(),
                WebkitTemporaryStorage = new DeprecatedStorageQuota(),
                Xr = new XRSystem()
            };
        }
    }
}
