using Microsoft.Extensions.Options;
using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace portfolio.infrastructure.ppi;

public class PpiGateway(HttpClient httpClient) : IPpiGateway
{
    private readonly HttpClient _httpClient = httpClient;
    readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public async Task<(IEnumerable<Position>, Ledger)> GetBalancesAndPositions(long accountNumber)
    {
        var endpoint = $"/api/1.0/Account/BalancesAndPositions?accountNumber={accountNumber}";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var dto = JsonSerializer.Deserialize<BalancesAndPositionsDTO>(content, options);

            var ledger = new Ledger { Name = domain.Constants.DataSources.PPI };

            if (dto is null)
                return ([], ledger);

            var positions = dto.GroupedInstruments
                               .SelectMany(group => group.Instruments
                               .Select(instrument => 
                                   new Position
                                   {
                                       DataSource = domain.Constants.DataSources.PPI,
                                       Symbol = instrument.Ticker,
                                       Exchange = string.Empty,
                                       AssetClass = group.Name,
                                       Currency = instrument.Currency switch
                                       {
                                           "Pesos" => domain.Constants.Currencies.ARS,
                                           _ => instrument.Currency,
                                       },
                                       Quantity = instrument.Quantity,
                                       Price = instrument.Price,
                                       MarketValue = instrument.Quantity * instrument.Price,
                                   }))
                               .ToList();

            ledger.Currencies = dto.GroupedAvailability
                                   .ToDictionary(
                                       group => group.Currency switch
                                       {
                                           "Pesos" => domain.Constants.Currencies.ARS,
                                           "Dolar Billete | MEP" => domain.Constants.Currencies.MEP,
                                           "Dolar Divisa | CCL" => domain.Constants.Currencies.USD,
                                           _ => group.Currency,
                                       },
                                       group => new Currency
                                       {
                                           Cash = group.Availability.Sum(a => a.Amount),
                                           BuyingPower = group.Availability.Sum(a => a.Amount),
                                       }
                                   );

            return (positions, ledger);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting balances and positions from PPI");
            throw;
        }
    }
}
