using portfolio.domain;
using portfolio.infrastructure.alpaca;
using portfolio.infrastructure.iol;

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

        app.MapGet("/positions", async (AlpacaGateway alpacaGateway, IolGateway iolGateway) =>
        {
            var alpacaPositionsTask = alpacaGateway.GetPositions();

            var iolPositionsTask = iolGateway.GetPositions();

            var result = await Task.WhenAll([alpacaPositionsTask, iolPositionsTask]);

            var positions = result.SelectMany(x => x);

            return Results.Ok(positions);
        })
        .WithOpenApi();

        app.MapGet("/ledger", async (AlpacaGateway alpacaGateway, IolGateway iolGateway) =>
        {
            var alpacaLedger = await alpacaGateway.GetLedger();

            var iolLedger = await iolGateway.GetLedger();

            var ledgers = new List<Ledger> { alpacaLedger, iolLedger };

            return Results.Ok(ledgers);
        })
        .WithOpenApi();

        return app;
    }
}
