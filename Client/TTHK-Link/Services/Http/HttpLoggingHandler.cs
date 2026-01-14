using System.Text;
using System.Text.Json;

namespace TTHK_Link.Services.Http;

public class HttpLoggingHandler : DelegatingHandler
{
    public HttpLoggingHandler()
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        System.Diagnostics.Debug.WriteLine("===== HTTP REQUEST =====");
        System.Diagnostics.Debug.WriteLine($"{request.Method} {request.RequestUri}");

        foreach (var h in request.Headers)
            System.Diagnostics.Debug.WriteLine($"H: {h.Key} = {string.Join(", ", h.Value)}");

        if (request.Content != null)
        {
            foreach (var h in request.Content.Headers)
                System.Diagnostics.Debug.WriteLine($"CH: {h.Key} = {string.Join(", ", h.Value)}");

            var reqBody = await request.Content.ReadAsStringAsync(cancellationToken);
            System.Diagnostics.Debug.WriteLine("Body:");
            System.Diagnostics.Debug.WriteLine(PrettyJsonOrRaw(reqBody));
        }

        var response = await base.SendAsync(request, cancellationToken);

        System.Diagnostics.Debug.WriteLine("===== HTTP RESPONSE =====");
        System.Diagnostics.Debug.WriteLine($"Status: {(int)response.StatusCode} {response.ReasonPhrase}");

        foreach (var h in response.Headers)
            System.Diagnostics.Debug.WriteLine($"H: {h.Key} = {string.Join(", ", h.Value)}");

        if (response.Content != null)
        {
            foreach (var h in response.Content.Headers)
                System.Diagnostics.Debug.WriteLine($"CH: {h.Key} = {string.Join(", ", h.Value)}");

            var respBody = await response.Content.ReadAsStringAsync(cancellationToken);
            System.Diagnostics.Debug.WriteLine("Body:");
            System.Diagnostics.Debug.WriteLine(PrettyJsonOrRaw(respBody));

            // Important: restore content so that ApiAuthService can read it again
            response.Content = new StringContent(
                respBody,
                Encoding.UTF8,
                response.Content.Headers.ContentType?.MediaType ?? "application/json"
            );
        }

        System.Diagnostics.Debug.WriteLine("========================");
        return response;
    }

    private static string PrettyJsonOrRaw(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "<empty>";

        try
        {
            using var doc = JsonDocument.Parse(s);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return s;
        }
    }
}
