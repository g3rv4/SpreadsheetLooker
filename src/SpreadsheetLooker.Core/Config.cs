using System.Collections.Immutable;
using System.IO;
using Google.Apis.Auth.OAuth2;

namespace SpreadsheetLooker.Core
{
    public class Config
    {
        public static Config Instance { get; private set; }
        
        public string SpreadsheetUrl { get; private set; }
        public ImmutableArray<string> CliFields { get; private set; }
        public ServiceAccountCredential AccountCredential { get; private set; }
        public string Token { get; private set; }
        
        public static void Init(string path)
        {
            if (Instance != null) return;
            
            var data = Jil.JSON.Deserialize<ConfigData>(File.ReadAllText(path));
            Instance = new Config
            {
                SpreadsheetUrl = data.SpreadsheetUrl,
                CliFields = data.CliFields.ToImmutableArray(),
                Token = data.Token,
                AccountCredential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(data.Email)
                {
                    Scopes = new[] { "https://www.googleapis.com/auth/spreadsheets.readonly"},
                    UseJwtAccessWithScopes = true,
                }.FromPrivateKey(data.PrivateKey)),
            };
        }

        private class ConfigData
        {
            public string Email { get; set; }
            public string PrivateKey { get; set; }
            public string SpreadsheetUrl { get; set; }
            public string[] CliFields { get; set; }
            public string Token { get; set; }
        }
    }
}