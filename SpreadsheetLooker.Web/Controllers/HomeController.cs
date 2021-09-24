using System.Linq;
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
            if (!Request.Headers.TryGetValue("Authorization", out var auth)
                || auth.FirstOrDefault() != "Bearer " + Config.Instance.Token)
            {
                return StatusCode(403);
            }
            var data = await GoogleSheetsHelper.GetDataAsync();
            return Content(JSON.Serialize(data), "application/json");
        }
    }
}
