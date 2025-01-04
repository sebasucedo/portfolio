using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public interface IAlpacaGateway
{
    Task<Ledger> GetLedger();
    Task<IEnumerable<Position>> GetPositions();
}
