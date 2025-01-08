using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure.ppi;

internal class PpiTokenService(HttpClient httpClient) : ITokenService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string> GetToken()
    {
        var url = "/api/1.0/Account/LoginApi";
        try
        {
            var dto = new { };
            string payload = JsonSerializer.Serialize(dto);
            StringContent content = new(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<PpiTokenResponse>(responseContent) ?? throw new InvalidOperationException();

            return token.AccessToken;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting token from Ppi");
            throw;
        }
    }
}
