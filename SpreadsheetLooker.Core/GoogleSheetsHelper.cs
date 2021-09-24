using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Jil;

namespace SpreadsheetLooker.Core
{
    public class GoogleSheetsHelper
    {
        public static async Task<ImmutableDictionary<string, string>> GetDataAsync()
        {
            var cred = Config.Instance.AccountCredential;
            var accessToken = await cred.GetAccessTokenForRequestAsync();

            var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                }
            };
            var res = await client.GetStringAsync(Config.Instance.RangeUrl);
            var data = JSON.Deserialize<SpreadsheetData>(res, Options.CamelCase);
            return data.Values.Where(v=>v.Length == 2)
                .ToImmutableDictionary(v => v[0], v => v[1]);
        }
        
        private class SpreadsheetData
        {
            public string[][] Values { get; private set; }
        }
    }
}