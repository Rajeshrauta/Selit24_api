using Microsoft.AspNetCore.Mvc;

namespace Selit24_webapp.Controllers
{
    public class ProductsController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:7273/api");

        private readonly HttpClient _client;

        public ProductsController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        
    }
}
