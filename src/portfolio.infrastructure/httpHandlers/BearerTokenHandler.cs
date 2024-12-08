using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.httpHandlers;

public class BearerTokenHandler(Func<string, ITokenService> tokenServiceSelector) : DelegatingHandler
{
    private readonly Func<string, ITokenService> _tokenServiceSelector = tokenServiceSelector ?? throw new ArgumentNullException(nameof(tokenServiceSelector));
    public required string Key { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Key))
            throw new ArgumentNullException(nameof(Key));

        var tokenService = _tokenServiceSelector(Key);
        var token = await tokenService.GetToken();
        request.Headers.Authorization = new AuthenticationHeaderValue(Constants.Keys.BEARER, token);

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        return httpResponseMessage;
    }
}
