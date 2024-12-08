using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure.alpaca;

public class AlpacaGateway(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public async Task<Ledger> GetLedger()
    {
        var endpoint = "/v2/account";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<AccountDTO>(content, options);

            var ledger = new Ledger { Name = "Alpaca" };
            if (dto is not null)
            {
                var usd = new Currency
                {
                    Cash = Convert.ToDecimal(dto.Cash),
                    BuyingPower = Convert.ToDecimal(dto.BuyingPower),
                };

                ledger.Currencies.Add(domain.Constants.Currencies.USD, usd);
            }

            return ledger;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting ledger from Alpaca");
            throw;
        }
    }

    public async Task<IEnumerable<Position>> GetPositions()
    {
        var endpoint = "/v2/positions";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<IEnumerable<PositionDTO>>(content, options);

            if (dto is null)
                return [];

            var positions = dto.Select(p => new Position
            { 
                Symbol = p.Symbol,
                Quantity = Convert.ToDecimal(p.Qty),
            });

            return positions;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting portfolio from Alpaca");
            throw;
        }
    }
}
