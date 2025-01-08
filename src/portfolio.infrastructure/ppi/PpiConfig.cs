using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure.ppi;

public class PpiConfig
{
    public required string BaseAddress { get; set; }
    public required string AuthorizedClient { get; set; }
    public required string ClientKey { get; set; }
    public required string ApiKey { get; set;}
    public required string ApiSecret { get; set;}
    public long AccountNumber { get; set; }
}
