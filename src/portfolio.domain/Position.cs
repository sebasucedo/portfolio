using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public class Position
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Symbol { get; set; }
    public required string Exchange { get; set; }
    public required string AssetClass { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal MarketValue { get; set; }

}
