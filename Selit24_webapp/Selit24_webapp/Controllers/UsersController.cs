using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Selit24_webapp.Models;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace Selit24_webapp.Controllers
{
    [Authorize(Roles ="User")]
    public class UsersController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7273/api");

        private readonly HttpClient _client;

        public UsersController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [Authorize]
        [Route("Users")]
        public IActionResult Users()
        {
            List<UserInfoModel> users = new List<UserInfoModel>();
            
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/UserAutho/GetActiveUsers").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserInfoModel>>(data) ;
            }
            return View(users);
        }
        [HttpGet]
        [Route("Users/Create")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Route("Users/Create")]
        public IActionResult CreateUser(UserInfoModel model)
        {
            try
            {
                using MultipartFormDataContent dataContent = new();

                // Add all non-file properties
                dataContent.Add(new StringContent(model.Firstname), "Firstname");
                dataContent.Add(new StringContent(model.Lastname), "Lastname");
                dataContent.Add(new StringContent(model.Locationcountry), "Locationcountry");
                dataContent.Add(new StringContent(model.Locationstate), "Locationstate");
                dataContent.Add(new StringContent(model.Locationdistrict), "Locationdistrict");
                dataContent.Add(new StringContent(model.Locationlocality), "Locationlocality");
                dataContent.Add(new StringContent(model.Locationpincode?.ToString()), "Locationpincode");
                dataContent.Add(new StringContent(model.Useremail), "Useremail");
                dataContent.Add(new StringContent(model.Userpassword), "Userpassword");

                // Add the file property (Profileimage)
                byte[] imageData;
                using (var ms = new MemoryStream())
                {
                    model.Profileimage.CopyTo(ms);
                    imageData = ms.ToArray();
                }

                var image = new StreamContent(new MemoryStream(imageData));
                image.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
                dataContent.Add(image, "Profileimage", model.Profileimage.FileName);

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/UserAutho/UserSignUp", dataContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product Created Successfully!!";
                    return RedirectToAction("Users");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        [HttpGet]
        [Route("Users/Details")]
        public IActionResult UserDetails(int id)
        {
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/UserAutho/GetActiveUserById/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<List<UserInfoModel>>(data).FirstOrDefault();
                if (user != null)
                {
                    return View(user);
                }
            }
            return View();
        }

        [HttpGet]
        [Route("Users/Edit/{ID?}")]
        public IActionResult EditUser(int id)
        {
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/UserAutho/GetActiveUserById/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var user = JsonConvert.DeserializeObject<List<UserInfoModel>>(data).FirstOrDefault();
                if (user != null)
                {
                    return View(user);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult DeleteUser(int id)
        {
            HttpResponseMessage response = _client.PutAsync($"{_client.BaseAddress}/UserAutho/UserDeleteById/{id}", null).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User Deleted Successfully!";
                return RedirectToAction("Users");
            }
            else
            {
                TempData["ErrorMessage"] = response.ReasonPhrase;
                return RedirectToAction("Users");
            }
        }

        [HttpPost]
        public IActionResult EditUser(UserInfoModel usermodel)
        {
            using MultipartFormDataContent dataContent = new();

            // Add all non-file properties
            dataContent.Add(new StringContent(usermodel.Userid.ToString()), "Userid");
            dataContent.Add(new StringContent(usermodel.Firstname), "Firstname");
            dataContent.Add(new StringContent(usermodel.Lastname), "Lastname");
            dataContent.Add(new StringContent(usermodel.Locationcountry), "Locationcountry");
            dataContent.Add(new StringContent(usermodel.Locationstate), "Locationstate");
            dataContent.Add(new StringContent(usermodel.Locationdistrict), "Locationdistrict");
            dataContent.Add(new StringContent(usermodel.Locationlocality), "Locationlocality");
            dataContent.Add(new StringContent(usermodel.Locationpincode?.ToString()), "Locationpincode");
            dataContent.Add(new StringContent(usermodel.Useremail), "Useremail");
            dataContent.Add(new StringContent(usermodel.Userpassword), "Userpassword");

            // Add the file property (Profileimage)
            byte[] imageData;
            using (var ms = new MemoryStream())
            {
                usermodel.Profileimage.CopyTo(ms);
                imageData = ms.ToArray();
            }

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            dataContent.Add(imageContent, "Profileimage", usermodel.Profileimage.FileName);

            HttpResponseMessage response = _client.PutAsync($"{_client.BaseAddress}/UserAutho/UpdateUserIfo/{usermodel.Userid}", dataContent).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User Deleted Successfully!";
                return RedirectToAction("Users");
            }
            else
            {
                TempData["ErrorMessage"] = response.ReasonPhrase;
                return RedirectToAction("Users");
            }
        }
    }
}
