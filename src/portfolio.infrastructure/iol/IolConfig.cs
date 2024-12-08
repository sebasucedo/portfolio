using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.iol;

public class IolConfig
{
    public required string BaseAddress { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
