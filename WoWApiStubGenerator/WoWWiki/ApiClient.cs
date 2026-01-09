using System;

namespace WoWApiStubGenerator.WoWWiki;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public partial class WoWWikiApiClient : BaseApiClient
{
    private static readonly string BASE_URL = "https://warcraft.wiki.gg";

    public WoWWikiApiClient(HttpClient? client = null) : base(BASE_URL, client)
    {
        
    }

    /// <summary>
    /// Returns the page wikitext (raw content) for the given page title.
    /// </summary>
    public async Task<string?> GetPageWikitextAsync(string title)
    {
        var qs = new Dictionary<string,string>{
            ["action"] = "query",
            ["titles"] = title,
            ["prop"] = "revisions",
            ["rvprop"] = "content",
            ["rvslots"] = "main",
            ["format"] = "json",
            ["formatversion"] = "2"
        };
        var query = string.Join('&', qs.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        var url = $"{BASE_URL}/api.php?{query}";

        var doc = await GetAsync<JsonDocument>(url).ConfigureAwait(false);
        if (doc == null) return null;
        var root = doc.RootElement;

        if (!root.TryGetProperty("query", out var queryElem)) return null;
        if (!queryElem.TryGetProperty("pages", out var pages) || pages.GetArrayLength() == 0) return null;
        var page = pages[0];
        if (!page.TryGetProperty("revisions", out var revs) || revs.GetArrayLength() == 0) return null;
        var rev = revs[0];
        if (!rev.TryGetProperty("slots", out var slots)) return null;
        if (!slots.TryGetProperty("main", out var main)) return null;
        if (!main.TryGetProperty("content", out var content)) return null;

        return content.ValueKind == JsonValueKind.String ? content.GetString() : content.ToString();
    }




}
