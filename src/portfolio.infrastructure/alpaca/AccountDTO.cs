using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portfolio.infrastructure.alpaca;

internal class AccountDTO
{
    public required string Cash { get; set; }
    [JsonPropertyName("Buying_Power")]
    public required string BuyingPower { get; set; }
}
