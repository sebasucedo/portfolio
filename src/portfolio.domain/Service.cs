using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace portfolio.domain;

public class Service(ICacheInterceptor cacheInterceptor,
                     IAlpacaGateway alpacaGateway,
                     IIolGateway iolGateway,
                     IPpiGateway ppiGateway)
{
    private readonly ICacheInterceptor _cacheInterceptor = cacheInterceptor;
    private readonly IAlpacaGateway _alpacaGateway = alpacaGateway;
    private readonly IIolGateway _iolGateway = iolGateway;
    private readonly IPpiGateway _ppiGateway = ppiGateway;

    public async Task<IEnumerable<Position>> GetPositions(long ppiAccountNumber)
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

        var ppiPositionsTask = _cacheInterceptor.ExecuteAsync(async () => //TODO fix this
                                    {
                                        var result = await _ppiGateway.GetBalancesAndPositions(ppiAccountNumber);
                                        return result.Item1;
                                    }, "/ppi_positions");

        var result = await Task.WhenAll([alpacaPositionsTask, iolPositionsTask, ppiPositionsTask]);
        var positions = result.SelectMany(x => x);
        return positions;
    }
}
