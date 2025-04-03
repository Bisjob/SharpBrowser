namespace SharpBrowser.BrowserObjects
{
    public class NavigatorUAData
    {
        public List<Brand> Brands { get; set; } = new List<Brand>
    {
        new Brand { BrandName = "Not-A.Brand", Version = "99" },
        new Brand { BrandName = "Chromium", Version = "124" }
    };
        public bool Mobile { get; set; } = true;
        public string Platform { get; set; } = "Android";
    }
}
