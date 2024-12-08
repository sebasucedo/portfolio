using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public class Position
{
    public required string Symbol { get; set; }
    public decimal Quantity { get; set; }
}
