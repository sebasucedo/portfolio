using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio.infrastructure;

public class Constants
{
    public static class EnvironmentVariables
    {
        public const string AWS_REGION = "REGION";
        public const string AWS_ACCESS_KEY = "ACCESS_KEY";
        public const string AWS_SECRET_KEY = "SECRET_KEY";
        public const string AWS_SECRET_NAME = "SECRET_NAME";
    }
    public static class SecretsManager
    {
        public const string AWSCURRENT = "AWSCURRENT";
    }

    public static class Keys
    {
        public const string ALPACA_KEY = "APCA-API-KEY-ID";
        public const string ALPACA_SECRET = "APCA-API-SECRET-KEY";
        public const string BEARER = "Bearer";
        public const string GRANT_TYPE_PASSWORD = "password";
    }
}
