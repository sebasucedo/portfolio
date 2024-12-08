using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.alpaca;

public class AlpacaConfig
{
    public required string BaseAddress { get; set; }
    public required string Key { get; set; }
    public required string Secret { get; set; }
}
