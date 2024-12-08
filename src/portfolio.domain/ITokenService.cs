using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public interface ITokenService
{
    Task<string> GetToken();
}
