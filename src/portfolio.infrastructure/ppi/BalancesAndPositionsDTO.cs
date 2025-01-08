using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.ppi;

internal class BalancesAndPositionsDTO
{
    public IEnumerable<AvailabilityGroup> GroupedAvailability { get; set; } = [];
    public IEnumerable<InstrumentGroup> GroupedInstruments { get; set; } = [];
}

internal class InstrumentGroup
{
    public required string Name { get; set; }
    public IEnumerable<Instrument> Instruments { get; set; } = [];
}

internal class Instrument
{
    public required string Ticker { get; set; }
    public required string Description { get; set; }
    public required string Currency { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal Quantity { get; set; }
    public decimal CollateralQuantity { get; set; }
}

internal class AvailabilityGroup
{
    public required string Currency { get; set; }
    public IEnumerable<Availability> Availability { get; set; } = [];
}

internal class Availability
{
    public required string Name { get; set; }
    public required string Symbol { get; set; }
    public decimal Amount { get; set; }
    public required string Settlement { get; set; }
}