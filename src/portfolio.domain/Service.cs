using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.domain;

public class Service(ICacheInterceptor cacheInterceptor,
                     IAlpacaGateway alpacaGateway,
                     IIolGateway iolGateway)
{
    private readonly ICacheInterceptor _cacheInterceptor = cacheInterceptor;
    private readonly IAlpacaGateway _alpacaGateway = alpacaGateway;
    private readonly IIolGateway _iolGateway = iolGateway;

    public async Task<IEnumerable<Position>> GetPositions()
    {
        var alpacaPositionsTask = _cacheInterceptor.ExecuteAsync(async () =>
                                    {
                                        var positions = await _alpacaGateway.GetPositions();
                                        return positions;
                                    }, "/alpaca_positions", 10);

        var iolPositionsTask = _cacheInterceptor.ExecuteAsync(async () =>
                                    {
                                        var positions = await _iolGateway.GetPositions();
                                        return positions;
                                    }, "/iol_positions");

        var result = await Task.WhenAll([alpacaPositionsTask, iolPositionsTask]);
        var positions = result.SelectMany(x => x);
        return positions;
    }
}
