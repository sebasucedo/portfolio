﻿using portfolio.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.infrastructure.iol;

public class IolGateway(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public async Task<Ledger> GetLedger()
    {
        var endpoint = "/api/estadocuenta";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<LedgerDTO>(content, options);

            var ledger = new Ledger { Name = "Iol" };
            if (dto is not null)
            {
                foreach (var cuenta in dto.Cuentas)
                {
                    var currency = new Currency
                    {
                        Cash = Convert.ToDecimal(cuenta.Saldo),
                        BuyingPower = Convert.ToDecimal(cuenta.Disponible),
                    };

                    var key = cuenta.Tipo switch
                    {
                        "inversion_Estados_Unidos_Dolares" => domain.Constants.Currencies.USD,
                        "inversion_Argentina_Dolares" => domain.Constants.Currencies.MEP,
                        "inversion_Argentina_Pesos" => domain.Constants.Currencies.ARS,
                        _ => throw new ArgumentException("tipo not found")
                    };

                    ledger.Currencies.Add(key, currency);
                }
            }

            return ledger;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting ledger from Iol");
            throw;
        }
    }

    public async Task<IEnumerable<Position>> GetPositions()
    {
        var endpoint = "/api/portafolio";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var portfolio = JsonSerializer.Deserialize<PortfolioDTO>(content, options);

            if (portfolio is null || portfolio.Activos is null)
                return [];

            var positions = portfolio.Activos.Select(x => new Position
            {
                Symbol = x.Titulo.Simbolo,
                Quantity = x.Cantidad,
            }).ToList();

            return positions;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "Error getting portfolio from Iol");
            throw;
        }
    }
}
