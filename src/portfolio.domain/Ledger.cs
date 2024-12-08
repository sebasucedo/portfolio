using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public class Ledger
{
    public required string Name { get; set; }
    public IDictionary<string, Currency> Currencies { get; set; } = new Dictionary<string, Currency>();
}

public class Currency
{
    public decimal Cash { get; set; }
    public decimal BuyingPower { get; set; }
}
