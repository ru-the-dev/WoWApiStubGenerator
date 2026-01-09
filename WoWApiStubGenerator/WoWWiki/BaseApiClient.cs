using System;
using System.Text;
using System.Text.Json;

namespace WoWApiStubGenerator.WoWWiki;
    
public class BaseApiClient : System.IDisposable
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public BaseApiClient(string baseUrl, HttpClient? client = null)
    {
        _baseUrl = baseUrl;
        _http = client ?? new HttpClient();
    }

    public void Dispose() => _http.Dispose();

    public async Task<T?> GetAsync<T>(string url)
    {
        using var resp = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        await using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<T>(stream).ConfigureAwait(false);
    }

    public async Task<TResponse?> PostJsonAsync<TRequest, TResponse>(string url, TRequest payload)
    {
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var resp = await _http.PostAsync(url, content).ConfigureAwait(false);
        resp.EnsureSuccessStatusCode();
        await using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        return await JsonSerializer.DeserializeAsync<TResponse>(stream).ConfigureAwait(false);
    }

    public void SetDefaultHeader(string name, string value)
    {
        if (_http.DefaultRequestHeaders.Contains(name))
            _http.DefaultRequestHeaders.Remove(name);
        _http.DefaultRequestHeaders.Add(name, value);
    }
}
