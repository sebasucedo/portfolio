using Microsoft.Extensions.Options;
using portfolio.domain;
using portfolio.infrastructure.ppi;

namespace portfolio.api;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {

        app.MapGet("/", () =>
        {
            return Results.Ok("My portfolio!");
        })
        .WithOpenApi();

        app.MapGet("/positions", async (Service service, IOptions<PpiConfig> config) =>
        {
            var ppiAccountNumber = config.Value.AccountNumber;
            var positions = await service.GetPositions(ppiAccountNumber);
            var apiResponse = new ApiResponse<IEnumerable<Position>>
            {
                Success = true,
                Data = positions,
            };
            return Results.Ok(apiResponse);
        })
        .WithOpenApi();

        app.MapGet("/ledger", async (IAlpacaGateway alpacaGateway, 
                                     IIolGateway iolGateway, 
                                     IPpiGateway ppiGateway,
                                     IOptions<PpiConfig> config) =>
        {
            var alpacaLedger = await alpacaGateway.GetLedger();

            var iolLedger = await iolGateway.GetLedger();

            var ppiAccountNumber = config.Value.AccountNumber;
            var ppiResult = await ppiGateway.GetBalancesAndPositions(ppiAccountNumber);
            var ppiLedger = ppiResult.Item2;

            var ledgers = new List<Ledger> { alpacaLedger, iolLedger, ppiLedger };
            var apiResponse = new ApiResponse<List<Ledger>>
            {
                Success = true,
                Data = ledgers,
            };
            return Results.Ok(apiResponse);
        })
        .WithOpenApi();

        return app;
    }
}
