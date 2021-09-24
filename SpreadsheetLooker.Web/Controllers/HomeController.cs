using System.Threading.Tasks;
using Jil;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLooker.Core;

namespace SpreadsheetLooker.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var data = await GoogleSheetsHelper.GetDataAsync();
            return Content(JSON.Serialize(data), "application/json");
        }
    }
}
