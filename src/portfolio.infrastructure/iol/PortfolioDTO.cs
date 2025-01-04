using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.iol;

internal class PortfolioDTO
{
    public int Pais { get; set; }
    public IEnumerable<PositionDTO> Activos { get; set; } = [];
}

internal class PositionDTO
{
    public decimal Cantidad { get; set; }
    public TituloDTO Titulo { get; set; } = null!;
    public decimal UltimoPrecio { get; set; }
    public decimal Valorizado { get; set; }
}

internal class TituloDTO
{
    public required string Simbolo { get; set; }
    public required string Mercado { get; set; }
    public required string Tipo { get; set; }
    public required string Moneda { get; set; }
}