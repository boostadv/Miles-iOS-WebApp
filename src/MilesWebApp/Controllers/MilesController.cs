using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using HtmlAgilityPack;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MilesWebApp.Controllers
{
    [Route("api/[controller]")]
    public class MilesController : Controller
    {
        // GET: api/values
        [HttpPost]
        public IActionResult GetMiles(UserModel user)
        {
            return new JsonResult(new {Miles = _loadData(user).Result });
        }

        public class UserModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        private async Task<string> _loadData(UserModel user)
        {
            string url = "http://www.iglobe.ru/cabinet_login?next=cabinet/booking";

            var formContent = new FormUrlEncodedContent(new[]
             {
                    new KeyValuePair<string, string>("site_auth", "sent"),
                    new KeyValuePair<string, string>("usr", user.Login),
                    new KeyValuePair<string, string>("pwd", user.Password)
            });


            HttpClient http = new HttpClient();

            var response = await http.PostAsync(url, formContent);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(await response.Content.ReadAsStringAsync());

            var milesDiv = doc.DocumentNode.Descendants().FirstOrDefault(c => c.Attributes.Contains("class") && c.Attributes["class"].Value.Contains("milesinfo-top-in"));

            if (milesDiv != null)
            {
                var milesCount = milesDiv.Descendants("strong").FirstOrDefault();

                return milesCount.InnerText;
            }
            else
            {
                return "Error";
            }
        }
    }
}
