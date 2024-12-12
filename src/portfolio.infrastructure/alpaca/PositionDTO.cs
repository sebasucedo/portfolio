using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace portfolio.infrastructure.alpaca;

internal class PositionDTO
{
    public required string Symbol { get; set; }
    public required string Exchange { get; set; }
    [JsonPropertyName("asset_class")]
    public required string AssetClass { get; set; }
    public required string Qty { get; set; }
    [JsonPropertyName("current_price")]
    public required string CurrentPrice { get; set; }
    [JsonPropertyName("market_value")]
    public required string MarketValue { get; set; }
}
