using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.iol;

internal class LedgerDTO
{
    public IEnumerable<CuentaDTO> Cuentas { get; set; } = [];
}

internal class CuentaDTO
{
    public required string Tipo { get; set; }
    public required string Moneda { get; set; }
    public decimal Saldo { get; set; }
    public decimal Comprometido { get; set; }
    public decimal Disponible { get; set; }
    public IEnumerable<SaldoDTO> Saldos { get; set; } = [];
}

internal class SaldoDTO
{
    public required string Liquidacion { get; set; }
    public decimal Saldo { get; set; }
    public decimal Comprometido { get; set; }
    public decimal Disponible { get; set; }
}
