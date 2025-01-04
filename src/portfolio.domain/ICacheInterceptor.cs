using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.domain;

public interface ICacheInterceptor
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> method, string key);
    Task<T> ExecuteAsync<T>(Func<Task<T>> method, string key, int durationMinutes);
}
