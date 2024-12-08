using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.alpaca;

internal class PositionDTO
{
    public required string Symbol { get; set; }
    public required string Qty { get; set; }
}
