namespace SharpBrowser.BrowserObjects
{
    public class NetworkInformation
    {
        public double Downlink { get; set; } = 1.45;
        public double DownlinkMax { get; set; } = double.MaxValue;
        public string EffectiveType { get; set; } = "4g";
        public Action OnChange { get; set; }
        public Action OnTypeChange { get; set; }
        public int Rtt { get; set; } = 0;
        public bool SaveData { get; set; } = false;
        public string Type { get; set; } = "wifi";
    }
}
