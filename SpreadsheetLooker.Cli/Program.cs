using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpreadsheetLooker.Core;

namespace SpreadsheetLooker.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Config.Init(args[0]);
            var data = await GoogleSheetsHelper.GetDataAsync();
            var selectedData = new List<string>();
            foreach (var key in Config.Instance.CliFields)
            {
                selectedData.Add(data[key]);
            }
            
            Console.WriteLine($"[ {string.Join(" I ", selectedData)} ]");
        }
    }
}
