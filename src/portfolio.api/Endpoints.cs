using portfolio.domain;

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

        app.MapGet("/positions", async (Service service) =>
        {
            var positions = await service.GetPositions();
            var apiResponse = new ApiResponse<IEnumerable<Position>>
            {
                Success = true,
                Data = positions,
            };
            return Results.Ok(apiResponse);
        })
        .WithOpenApi();

        app.MapGet("/ledger", async (IAlpacaGateway alpacaGateway, IIolGateway iolGateway) =>
        {
            var alpacaLedger = await alpacaGateway.GetLedger();

            var iolLedger = await iolGateway.GetLedger();

            var ledgers = new List<Ledger> { alpacaLedger, iolLedger };
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
