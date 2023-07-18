using System.Linq;
using System.Threading.Tasks;
using Jil;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLooker.Core;

namespace SpreadsheetLooker.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        [Route("{sheet}", Order = 1)]
        public async Task<IActionResult> Index(string sheet = null)
        {
            if (!Request.Headers.TryGetValue("Authorization", out var auth)
                || auth.FirstOrDefault() != "Bearer " + Config.Instance.Token)
            {
                return StatusCode(403);
            }
            var data = await GoogleSheetsHelper.GetDataAsync(sheet);
            return Content(JSON.Serialize(data), "application/json");
        }
    }
}
