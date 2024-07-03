using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Selit24_webapp.Models;

namespace Selit24_webapp.Controllers
{
    public class CustomerController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7273/api");

        private readonly HttpClient _client;

        public CustomerController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult CustomerList()
        {
            List<CustomerInfoModel> users = new List<CustomerInfoModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/CustomerAutho/GetActiveCustomers").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<CustomerInfoModel>>(data);
            }
            return View(users);
        }
    }
}
