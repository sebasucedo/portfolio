using Microsoft.Extensions.Options;
using portfolio.domain;
using portfolio.infrastructure.iol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure.invertironline;

public class IolTokenService(HttpClient httpClient, IOptions<IolConfig> config) : ITokenService
{
    const string REQUEST_URI = "/token";

    private readonly HttpClient _httpClient = httpClient;
    private readonly IolConfig _config = config.Value;
    private IolTokenResponse? _cachedToken;

    public async Task<string> GetToken()
    {
        if (_cachedToken != null && !_cachedToken.HasExpired) //TODO no funciona cache todavía ya que no es singleton
            return _cachedToken.AccessToken;

        var response = await _httpClient.PostAsync(REQUEST_URI, new FormUrlEncodedContent(
                                new Dictionary<string, string>
                                    {
                                        { "username", _config.Username },
                                        { "password",  _config.Password },
                                        { "grant_type", Constants.Keys.GRANT_TYPE_PASSWORD },
                                    }));

        var content = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<IolTokenResponse>(content) ?? throw new InvalidOperationException();

        _cachedToken = token;

        return token.AccessToken;
    }
}
