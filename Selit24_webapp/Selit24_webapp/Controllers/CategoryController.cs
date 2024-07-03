using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Selit24_webapp.Models;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace Selit24_webapp.Controllers
{
    public class CategoryController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7273/api");

        private readonly HttpClient _client;

        public CategoryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Categories()
        {
            List<CategoryModel> category = new List<CategoryModel>();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/ProductCategory/GetAllCategories").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                category = JsonConvert.DeserializeObject<List<CategoryModel>>(data);
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel categoryModel)
        {
            try
            {
                using MultipartFormDataContent dataContent = new();

                // Add all non-file properties
                dataContent.Add(new StringContent(categoryModel.Categoryname), "Categoryname");

                byte[] imageBytes = null;
                if (categoryModel.Categoryimage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        categoryModel.Categoryimage.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                var image = new StreamContent(new MemoryStream(imageBytes));
                image.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
                dataContent.Add(image, "Categoryimage", categoryModel.Categoryimage.FileName);

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "/ProductCategory/CreateCategory", dataContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category Created Successfully!!";
                    return RedirectToAction("Categories");
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
        public IActionResult UpdateCategory(int id)
        {

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + $"/ProductCategory/GetCategoryById/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var category = JsonConvert.DeserializeObject<List<CategoryModel>>(data).FirstOrDefault();

                if (category != null)
                {
                    return View(category);
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult UpdateCategory(CategoryModel categoryModel)
        {
            try
            {
                using MultipartFormDataContent dataContent = new();

                // Add all non-file properties
                dataContent.Add(new StringContent(categoryModel.Categoryname), "Categoryname");
                dataContent.Add(new StringContent(categoryModel.Categoryid.ToString()), "Categoryid");

                byte[] imageBytes = null;
                if (categoryModel.Categoryimage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        categoryModel.Categoryimage.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                if(categoryModel.Categoryimage != null) 
                {
                    var image = new StreamContent(new MemoryStream(imageBytes));
                    image.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Image.Jpeg);
                    dataContent.Add(image, "Categoryimage", categoryModel.Categoryimage.FileName);
                }

                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/ProductCategory/UpdateProductCategory", dataContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Category Created Successfully!!";
                    return RedirectToAction("Categories");
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
        public IActionResult DeleteCategory(int id)
        {
            HttpResponseMessage response = _client.DeleteAsync($"{_client.BaseAddress}/ProductCategory/DeleteCategory/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category Deleted Successfully!";
                return RedirectToAction("Categories");
            }
            else
            {
                TempData["ErrorMessage"] = response.ReasonPhrase;
                return RedirectToAction("Categories");
            }
        }
    }
}
