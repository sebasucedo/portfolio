using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portfolio.infrastructure.invertironline;

public record IolTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresIn,
    [property: JsonPropertyName("refresh_token")] string RefreshToken)
{
    public DateTime ObtainedAt { get; set; } = DateTime.UtcNow;

    public bool HasExpired => DateTime.UtcNow > ObtainedAt.AddSeconds(ExpiresIn - 30);
}
