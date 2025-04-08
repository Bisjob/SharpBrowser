using Jint;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Debugger;
using SharpBrowser.BrowserObjects;

namespace SharpBrowser
{
    public class BrowserOptions
    {
        public bool AutoExecuteScript { get; set; }
    }

    public class Browser
    {
        public event EventHandler PageHasChanged;
        public event EventHandler UrlHasChanged;

        public BrowserOptions Options { get; }
        public Engine Engine { get; }
        public Window Window { get; }
        public HTMLDocument Document => Window.Document;
        public ConsoleObject ConsoleObj => Window.ConsoleObj;
        public LocationObject Location => Window.Location;

        public IEnumerable<string> Errors => errors;
        public IEnumerable<string> ScriptsToExecute => scriptsToExecute;

        private readonly HttpClient client;

        private readonly List<string> errors = [];
        private readonly List<string> scriptsToExecute = [];
        private readonly Dictionary<string, string> sideStylesheets = [];

        public Browser(HttpClient client, BrowserOptions options)
        {
            this.client = client;
            Options = options;
            Engine = new Engine(o => o.DebugMode().InitialStepMode(StepMode.Into));


            Engine.Debugger.Step += (sender, info) =>
            {
                if (info.CurrentNode != null)
                {
                    //Console.WriteLine($"STEP {step++} in {info.CurrentNode.Start}-{info.CurrentNode.End}");
                }
                return StepMode.Into;
            };
            Engine.Debugger.BeforeEvaluate += (sender, info) =>
            {

            };

            Window = new Window(Engine, client.DefaultRequestHeaders.UserAgent.ToString());

            Window.Document.HasChanged += (s, e) =>
            {
                PageHasChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        public async Task NavigateTo(string url)
        {
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            SetUrl(url);
            await SetHtmlContent(content);
        }

        public void SetUrl(string url)
        {
            Location.SetUrl(url);
            UrlHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task SetHtmlContent(string html)
        {
            errors.Clear();
            Document.SetHTMLContent(html);
            await LoadStylesheets();
            await LoadScripts();
            Document.DispatchEvent("DOMContentLoaded", Array.Empty<JsValue>());
            PageHasChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool ExecuteScript(string script)
        {
            if (string.IsNullOrEmpty(script)) return true;

            try
            {
                Engine.Execute(script);
                return true;
            }
            catch (JavaScriptException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                AddError($"Script failed to execute: {ex.Message}");
                return false;
            }
        }


        public Uri GetUri()
            => Location.CurrentUri;

        public string GetPageTitle()
        {
            var titleTag = Document.GetElementsByTagName("title");
            if (!titleTag.Any())
                return string.Empty;

            return titleTag.First().InnerHTML;
        }
        public string GetHtmlContent()
            => Document.ToString();
        public string GetBodyContent()
            => Document.Body?.ToString();
        public string GetStyleContent()
        {
            var result = string.Empty;
            result += string.Join('\n', sideStylesheets.Values.Select(x => x));
            var styles = Document.Styles;
            if (styles?.Any() == true)
                result += "\n" + string.Join("\n", styles.Select(x => x.InnerHTML));
            return result;
        }

        private async Task LoadStylesheets()
        {
            var links = Document.GetElementsByTagName("link")
                            .Where(x => x.Attributes.Any(
                                at => at.Key == "rel" &&
                                (at.Value == "stylesheet" ||
                                (at.Value == "preload" && x.Attributes.Any(at2 => at2.Key == "as" && at2.Value == "style")))))
                            .ToList();
            foreach (var link in links)
            {
                var href = link.GetAttribute("href");
                if (!string.IsNullOrEmpty(href))
                {
                    if (!href.StartsWith("http"))
                    {
                        var uri = new Uri(Location.CurrentUri, href);
                        href = uri.ToString();
                    }
                    var content = await GetRequestContent(href);
                    if (!string.IsNullOrEmpty(content))
                        sideStylesheets[href] = content;
                }
            }
        }
        private async Task LoadScripts()
        {
            scriptsToExecute.Clear();
            // Execute blocking scripts in the head
            var blockingHeadScripts = Document.Head.Children.Where(x => x.TagName == "script")
            .Where(s => s.Attributes.ContainsKey("src") && !s.Attributes.ContainsKey("async") && !s.Attributes.ContainsKey("defer"))
            .ToList();
            await ExecuteScriptsAsync(blockingHeadScripts);

            var scripts = Document.GetElementsByTagName("script").ToList();

            // Execute async scripts 
            var asyncScripts = scripts.Where(s => s.Attributes.ContainsKey("async")).ToList();
            await ExecuteScriptsAsync(asyncScripts);

            // Execute deferred scripts
            var deferredScripts = scripts.Where(s => s.Attributes.ContainsKey("defer")).ToList();
            await ExecuteScriptsAsync(deferredScripts);

            // Execute scripts in the body
            var bodyScripts = Document.Body.GetElementsByTagName("script").ToList();
            await ExecuteScriptsAsync(bodyScripts);

        }
        private async Task ExecuteScriptsAsync(IEnumerable<HTMLElement> scripts)
        {
            foreach (var script in scripts)
            {
                await ExecuteScriptAsync(script);
            }
        }
        private async Task ExecuteScriptAsync(HTMLElement script)
        {
            if (script.HasAttribute("src"))
            {
                var scriptUrl = script.GetAttribute("src");
                if (!scriptUrl.StartsWith("http"))
                {
                    var uri = new Uri(Location.CurrentUri, scriptUrl);
                    scriptUrl = uri.ToString();
                }
                var scriptContent = await GetRequestContent(scriptUrl);

                if (!Options.AutoExecuteScript)
                    scriptsToExecute.Add(scriptContent);
                else
                    ExecuteScript(scriptContent);
            }
            else
            {

                if (!Options.AutoExecuteScript)
                    scriptsToExecute.Add(script.InnerHTML);
                else
                    ExecuteScript(script.InnerHTML);
            }
        }

        private void AddError(string message)
        {
            errors.Add(message);
        }

        private async Task<string> GetRequestContent(string url)
        {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Failed to fetch content from {url}: {e.Message}");
                return null;
            }

        }
    }
}
