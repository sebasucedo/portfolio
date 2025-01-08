using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portfolio.infrastructure.ppi;

public record PpiTokenResponse(
    [property: JsonPropertyName("creationDate")] DateTime CreationDate,
    [property: JsonPropertyName("expirationDate")] DateTime ExpirationDate,
    [property: JsonPropertyName("accessToken")] string AccessToken,
    [property: JsonPropertyName("refreshToken")] string RefreshToken);